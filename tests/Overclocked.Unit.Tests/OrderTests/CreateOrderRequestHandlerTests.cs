using System.Reflection;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Factories;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Common.Constants.Payment;
using Overclocked.Application.Features.OrderUseCases.CreateOrder;
using Overclocked.Application.Features.OrderUseCases.DTOs.Requests;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.CartAggregate;
using Overclocked.Domain.Common.Shared.ValueObjects.Money;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.PaymentAggregate;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Exceptions;
using Polly;
using Polly.Registry;
using Shouldly;

namespace Overclocked.Unit.Tests.OrderTests;

public class CreateOrderRequestHandlerTests
{
    private readonly IOrderRepository _orderRepositoryMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IProductRepository _productRepositoryMock;
    private readonly ICartRepository _cartRepositoryMock;
    private readonly IPaymentRepository _paymentRepositoryMock;
    private readonly PaymentFactory _paymentFactoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ResiliencePipelineProvider<string> _pipelineProviderMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly CreateOrderRequestHandler _handler;

    public CreateOrderRequestHandlerTests()
    {
        _orderRepositoryMock = Substitute.For<IOrderRepository>();
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _productRepositoryMock = Substitute.For<IProductRepository>();
        _cartRepositoryMock = Substitute.For<ICartRepository>();
        _paymentRepositoryMock = Substitute.For<IPaymentRepository>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _pipelineProviderMock = Substitute.For<ResiliencePipelineProvider<string>>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();

        var stripeProvider = Substitute.For<IPaymentProviderService>();
        stripeProvider.PaymentProvider.Returns(PaymentProvider.Paymob);
        var walletProvider = Substitute.For<IPaymentProviderService>();
        walletProvider.PaymentProvider.Returns(PaymentProvider.Internal);

        _paymentFactoryMock = new PaymentFactory(new List<IPaymentProviderService> { stripeProvider, walletProvider });

        _pipelineProviderMock.GetPipeline(ResilienceConstants.StandardPolicy).Returns(ResiliencePipeline.Empty);

        _handler = new CreateOrderRequestHandler(
            _orderRepositoryMock,
            _userRepositoryMock,
            _productRepositoryMock,
            _cartRepositoryMock,
            _paymentRepositoryMock,
            _paymentFactoryMock,
            _unitOfWorkMock,
            _pipelineProviderMock
        );
    }

    [Fact]
    public async Task CreateOrderRequestHandler_Should_ReturnFailure_When_AddressIsInvalid()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            UserId = Guid.NewGuid(),
            ShippingAddress = new ShippingAddressRequestDto(0, "Building", "Street", "City", "12345", "Description"),
            PaymentMethod = PaymentMethod.Balance,
            PaymentProvider = PaymentProvider.Internal,
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Validation);
    }

    [Fact]
    public async Task CreateOrderRequestHandler_Should_ThrowCartNotFoundException_When_CartDoesNotExist()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            UserId = Guid.NewGuid(),
            ShippingAddress = new ShippingAddressRequestDto(1, "Building", "Street", "City", "12345", "Description"),
            PaymentMethod = PaymentMethod.Balance,
            PaymentProvider = PaymentProvider.Internal,
        };

        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns((Cart)null!);

        // Act
        CartNotFoundException exception = await Should.ThrowAsync<CartNotFoundException>(async () =>
            await _handler.Handle(request, CancellationToken.None)
        );

        // Assert
        exception.Message.ShouldContain(request.UserId.ToString());
    }

    [Fact]
    public async Task CreateOrderRequestHandler_Should_ReturnFailure_When_CartIsEmpty()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            UserId = Guid.NewGuid(),
            ShippingAddress = new ShippingAddressRequestDto(1, "Building", "Street", "City", "12345", "Description"),
            PaymentMethod = PaymentMethod.Balance,
            PaymentProvider = PaymentProvider.Internal,
        };

        var userId = UserId.Create(request.UserId);
        var cart = Cart.Create(userId);
        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(cart);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(OrderErrors.EmptyCart);
    }

    [Fact]
    public async Task CreateOrderRequestHandler_Should_ReturnFailure_When_ProductNotFound()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            UserId = Guid.NewGuid(),
            ShippingAddress = new ShippingAddressRequestDto(1, "Building", "Street", "City", "12345", "Description"),
            PaymentMethod = PaymentMethod.Balance,
            PaymentProvider = PaymentProvider.Internal,
        };
        var userId = UserId.Create(request.UserId);
        var productId = ProductId.Create(Guid.NewGuid());
        var cart = Cart.Create(userId);
        cart.AddCartItem(productId, 1);
        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(cart);
        _productRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>()).Returns([]);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
    }

    [Fact]
    public async Task CreateOrderRequestHandler_Should_ReturnFailure_When_StockIsInsufficient()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            UserId = Guid.NewGuid(),
            ShippingAddress = new ShippingAddressRequestDto(1, "Building", "Street", "City", "12345", "Description"),
            PaymentMethod = PaymentMethod.Balance,
            PaymentProvider = PaymentProvider.Internal,
        };
        var userId = UserId.Create(request.UserId);
        var product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();
        // Set stock to 0
        typeof(Product)
            .GetProperty(nameof(Product.StockQuantity), BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(product, 0);

        var cart = Cart.Create(userId);
        cart.AddCartItem(product.Id, 1);
        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(cart);
        _productRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>()).Returns([product]);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
    }

    [Fact]
    public async Task CreateOrderRequestHandler_Should_ReturnFailure_When_PaymentMethodIsBalance_AndInsufficientFunds()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            UserId = Guid.NewGuid(),
            ShippingAddress = new ShippingAddressRequestDto(1, "Building", "Street", "City", "12345", "Description"),
            PaymentMethod = PaymentMethod.Balance,
            PaymentProvider = PaymentProvider.Internal,
        };
        var userId = UserId.Create(request.UserId);
        var product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();
        typeof(Product)
            .GetProperty(nameof(Product.Price), BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(product, Money.Create(100m).Value);

        var cart = Cart.Create(userId);
        cart.AddCartItem(product.Id, 1);

        var user = new UserFaker(_passwordHasherMock).Generate();
        typeof(User).GetProperty(nameof(User.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(user, userId);
        typeof(User)
            .GetProperty(nameof(User.Balance), BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(user, Money.Create(50m).Value);

        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(cart);
        _productRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>()).Returns([product]);
        _userRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldBe(OrderErrors.InsufficientBalance);
    }

    [Fact]
    public async Task CreateOrderRequestHandler_Should_Succeed_When_PaymentMethodIsBalance_AndFundsAreSufficient()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            UserId = Guid.NewGuid(),
            ShippingAddress = new ShippingAddressRequestDto(1, "Building", "Street", "City", "12345", "Description"),
            PaymentMethod = PaymentMethod.Balance,
            PaymentProvider = PaymentProvider.Internal,
        };
        var userId = UserId.Create(request.UserId);
        var product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();
        typeof(Product)
            .GetProperty(nameof(Product.Price), BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(product, Money.Create(100m).Value);

        var cart = Cart.Create(userId);
        cart.AddCartItem(product.Id, 1);

        var user = new UserFaker(_passwordHasherMock).Generate();
        typeof(User).GetProperty(nameof(User.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(user, userId);
        typeof(User)
            .GetProperty(nameof(User.Balance), BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(user, Money.Create(200m).Value);

        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(cart);
        _productRepositoryMock
            .GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product });
        _userRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        user.Balance.Value.ShouldBe(100m);
        _orderRepositoryMock.Received(1).Add(Arg.Any<Order>());
        _paymentRepositoryMock.Received(1).Add(Arg.Any<Payment>());
        cart.CartItems.ShouldBeEmpty();
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateOrderRequestHandler_Should_Succeed_When_PaymentMethodIsCOD()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            UserId = Guid.NewGuid(),
            ShippingAddress = new ShippingAddressRequestDto(1, "Building", "Street", "City", "12345", "Description"),
            PaymentMethod = PaymentMethod.CashOnDelivery,
            PaymentProvider = PaymentProvider.Internal,
        };
        request = request with { PaymentMethod = PaymentMethod.CashOnDelivery, PaymentProvider = PaymentProvider.Internal };
        var userId = UserId.Create(request.UserId);
        var product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        var cart = Cart.Create(userId);
        cart.AddCartItem(product.Id, 1);

        var user = new UserFaker(_passwordHasherMock).Generate();
        typeof(User).GetProperty(nameof(User.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(user, userId);

        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(cart);
        _productRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>()).Returns([product]);
        _userRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        _orderRepositoryMock.Received(1).Add(Arg.Is<Order>(o => o.Status == OrderStatus.Placed));
        cart.CartItems.ShouldBeEmpty();
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CreateOrderRequestHandler_Should_ReturnFailure_When_ProviderUrlGenerationFails()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            UserId = Guid.NewGuid(),
            ShippingAddress = new ShippingAddressRequestDto(1, "Building", "Street", "City", "12345", "Description"),
            PaymentMethod = PaymentMethod.CreditCard,
            PaymentProvider = PaymentProvider.Paymob,
        };
        request = request with { PaymentMethod = PaymentMethod.CreditCard, PaymentProvider = PaymentProvider.Paymob };
        var userId = UserId.Create(request.UserId);
        var product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        var cart = Cart.Create(userId);
        cart.AddCartItem(product.Id, 1);

        var user = new UserFaker(_passwordHasherMock).Generate();
        typeof(User).GetProperty(nameof(User.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(user, userId);

        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(cart);
        _productRepositoryMock
            .GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product });
        _userRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        var providerMock = _paymentFactoryMock.GetProvider(PaymentProvider.Paymob);
        providerMock
            .GeneratePaymentUrl(Arg.Any<Order>(), Arg.Any<User>(), Arg.Any<PaymentMethod>(), Arg.Any<CancellationToken>())
            .Returns(Result.Failure<string>(Error.Failure("Payment.UrlGeneration", "Failed")));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.Code.ShouldBe("Payment.UrlGeneration");
    }

    [Fact]
    public async Task CreateOrderRequestHandler_Should_Succeed_When_PaymentMethodIsOnline()
    {
        // Arrange
        var request = new CreateOrderRequest
        {
            UserId = Guid.NewGuid(),
            ShippingAddress = new ShippingAddressRequestDto(1, "Building", "Street", "City", "12345", "Description"),
            PaymentMethod = PaymentMethod.CreditCard,
            PaymentProvider = PaymentProvider.Paymob,
        };
        request = request with { PaymentMethod = PaymentMethod.CreditCard, PaymentProvider = PaymentProvider.Paymob };
        var userId = UserId.Create(request.UserId);
        var product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();

        var cart = Cart.Create(userId);
        cart.AddCartItem(product.Id, 1);

        var user = new UserFaker(_passwordHasherMock).Generate();
        typeof(User).GetProperty(nameof(User.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(user, userId);

        _cartRepositoryMock.GetAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(cart);
        _productRepositoryMock
            .GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>())
            .Returns(new List<Product> { product });
        _userRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        var providerMock = _paymentFactoryMock.GetProvider(PaymentProvider.Paymob);
        providerMock
            .GeneratePaymentUrl(Arg.Any<Order>(), Arg.Any<User>(), Arg.Any<PaymentMethod>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success("https://payment.url"));

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.RedirectUrl.ShouldBe("https://payment.url");
        result.Value.PaymentPending.ShouldBeTrue();
        cart.CartItems.ShouldBeEmpty();
        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
