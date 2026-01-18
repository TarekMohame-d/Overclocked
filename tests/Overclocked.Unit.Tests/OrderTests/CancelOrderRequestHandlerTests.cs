// using System.Reflection;
// using NSubstitute;
// using Overclocked.Application.Abstractions;
// using Overclocked.Application.Abstractions.Factories;
// using Overclocked.Application.Abstractions.Persistence;
// using Overclocked.Application.Abstractions.Services;
// using Overclocked.Application.Common.Constants.Payment;
// using Overclocked.Application.Features.OrderUseCases.CancelOrder;
// using Overclocked.Architecture.Tests.FakeData;
// using Overclocked.Domain.Common.Shared.ValueObjects.Address;
// using Overclocked.Domain.Common.Shared.ValueObjects.Money;
// using Overclocked.Domain.OrderAggregate;
// using Overclocked.Domain.OrderAggregate.ValueObjects;
// using Overclocked.Domain.PaymentAggregate;
// using Overclocked.Domain.PaymentAggregate.ValueObjects;
// using Overclocked.Domain.ProductAggregate;
// using Overclocked.Domain.ProductAggregate.ValueObjects;
// using Overclocked.Domain.UserAggregate;
// using Overclocked.Domain.UserAggregate.ValueObjects;
// using Overclocked.SharedKernel;
// using Shouldly;

// namespace Overclocked.Unit.Tests.OrderTests;

// public class CancelOrderRequestHandlerTests
// {
//     private readonly IOrderRepository _orderRepositoryMock;
//     private readonly IUserRepository _userRepositoryMock;
//     private readonly IPaymentRepository _paymentRepositoryMock;
//     private readonly IProductRepository _productRepositoryMock;
//     private readonly IUnitOfWork _unitOfWorkMock;
//     private readonly PaymentFactory _paymentFactoryMock;
//     private readonly IPasswordHasher _passwordHasherMock;
//     private readonly CancelOrderRequestHandler _handler;

//     public CancelOrderRequestHandlerTests()
//     {
//         _orderRepositoryMock = Substitute.For<IOrderRepository>();
//         _userRepositoryMock = Substitute.For<IUserRepository>();
//         _paymentRepositoryMock = Substitute.For<IPaymentRepository>();
//         _productRepositoryMock = Substitute.For<IProductRepository>();
//         _unitOfWorkMock = Substitute.For<IUnitOfWork>();
//         _passwordHasherMock = Substitute.For<IPasswordHasher>();

//         var stripeProvider = Substitute.For<IPaymentProviderService>();
//         stripeProvider.PaymentProvider.Returns(PaymentProvider.Paymob);
//         var walletProvider = Substitute.For<IPaymentProviderService>();
//         walletProvider.PaymentProvider.Returns(PaymentProvider.Internal);

//         _paymentFactoryMock = new PaymentFactory(new List<IPaymentProviderService> { stripeProvider, walletProvider });

//         _handler = new CancelOrderRequestHandler(
//             _orderRepositoryMock,
//             _userRepositoryMock,
//             _paymentRepositoryMock,
//             _productRepositoryMock,
//             _unitOfWorkMock,
//             _paymentFactoryMock
//         );
//     }

//     [Fact]
//     public async Task Handle_ShouldReturnFailure_WhenOrderNotFound()
//     {
//         // Arrange
//         var request = new CancelOrderRequest { UserId = Guid.NewGuid(), OrderId = Guid.NewGuid(), RefundToWallet = true };
//         _orderRepositoryMock.GetByIdAsync(Arg.Any<OrderId>(), Arg.Any<CancellationToken>()).Returns((Order)null!);

//         // Act
//         var result = await _handler.Handle(request, CancellationToken.None);

//         // Assert
//         result.IsFailure.ShouldBeTrue();
//         result.Error.Code.ShouldBe("Order.NotFound");
//     }

//     [Fact]
//     public async Task Handle_ShouldReturnFailure_WhenOrderAlreadyCancelled()
//     {
//         // Arrange
//         var orderId = Guid.NewGuid();
//         var request = new CancelOrderRequest { UserId = Guid.NewGuid(), OrderId = orderId, RefundToWallet = true };
//         var order = CreateOrder();
//         typeof(Order).GetProperty(nameof(Order.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(order, OrderId.Create(orderId));
//         order.MarkAsCancelled();
//         _orderRepositoryMock.GetByIdAsync(Arg.Is<OrderId>(id => id.Value == orderId), Arg.Any<CancellationToken>()).Returns(order);

//         // Act
//         var result = await _handler.Handle(request, CancellationToken.None);

//         // Assert
//         result.IsFailure.ShouldBeTrue();
//         result.Error.ShouldBe(OrderErrors.OrderAlreadyCancelled);
//     }

//     [Fact]
//     public async Task Handle_ShouldReturnFailure_WhenOrderIsExpired()
//     {
//         // Arrange
//         var orderId = Guid.NewGuid();
//         var request = new CancelOrderRequest { UserId = Guid.NewGuid(), OrderId = orderId, RefundToWallet = true };
//         var order = CreateOrder();
//         typeof(Order).GetProperty(nameof(Order.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(order, OrderId.Create(orderId));
//         typeof(Order).GetProperty(nameof(Order.CreatedAt), BindingFlags.Public | BindingFlags.Instance)!.SetValue(order, DateTimeOffset.UtcNow.AddMinutes(-31));
//         _orderRepositoryMock.GetByIdAsync(Arg.Is<OrderId>(id => id.Value == orderId), Arg.Any<CancellationToken>()).Returns(order);

//         // Act
//         var result = await _handler.Handle(request, CancellationToken.None);

//         // Assert
//         result.IsFailure.ShouldBeTrue();
//         result.Error.ShouldBe(OrderErrors.CanNotCancel);
//     }

//     [Fact]
//     public async Task Handle_ShouldReturnFailure_WhenStatusIsInvalid()
//     {
//         // Arrange
//         var orderId = Guid.NewGuid();
//         var request = new CancelOrderRequest { UserId = Guid.NewGuid(), OrderId = orderId, RefundToWallet = true };
//         var order = CreateOrder();
//         typeof(Order).GetProperty(nameof(Order.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(order, OrderId.Create(orderId));
//         order.MarkAsProcessing();
//         _orderRepositoryMock.GetByIdAsync(Arg.Is<OrderId>(id => id.Value == orderId), Arg.Any<CancellationToken>()).Returns(order);

//         // Act
//         var result = await _handler.Handle(request, CancellationToken.None);

//         // Assert
//         result.IsFailure.ShouldBeTrue();
//         result.Error.ShouldBe(OrderErrors.CanNotCancel);
//     }

//     [Fact]
//     public async Task Handle_ShouldSucceed_WhenOrderIsUnpaid()
//     {
//         // Arrange
//         var orderId = Guid.NewGuid();
//         var request = new CancelOrderRequest { UserId = Guid.NewGuid(), OrderId = orderId, RefundToWallet = true };
//         var order = CreateOrder();
//         typeof(Order).GetProperty(nameof(Order.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(order, OrderId.Create(orderId));
//         var product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();
//         order.AddItem(product.Id, product.Name, product.Thumbnail, 100m, 1);
//         _orderRepositoryMock.GetByIdAsync(Arg.Is<OrderId>(id => id.Value == orderId), Arg.Any<CancellationToken>()).Returns(order);

//         var payment = Payment.Create(order.Id, PaymentProvider.Paymob.ToString(), PaymentMethod.CreditCard.ToString(), order.CalculateTotalPrice());
//         _paymentRepositoryMock.GetByOrderIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(payment);
//         _productRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>())
//             .Returns(new List<Product> { product });

//         // Act
//         var result = await _handler.Handle(request, CancellationToken.None);

//         // Assert
//         result.IsSuccess.ShouldBeTrue();
//         order.Status.ShouldBe(OrderStatus.Cancelled);
//         payment.Status.ShouldBe(PaymentStatus.Cancelled);
//         product.StockQuantity.ShouldBe(1); // Default is 0, then AddStock(1)
//         await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
//     }

//     [Fact]
//     public async Task Handle_ShouldSucceed_WhenRefundingToWallet()
//     {
//         // Arrange
//         var userId = UserId.Create(Guid.NewGuid());
//         var orderId = Guid.NewGuid();
//         var request = new CancelOrderRequest { UserId = userId.Value, OrderId = orderId, RefundToWallet = true };
//         var order = CreateOrder(userId);
//         typeof(Order).GetProperty(nameof(Order.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(order, OrderId.Create(orderId));
//         var product = new ProductFaker(Guid.NewGuid(), Guid.NewGuid()).Generate();
//         order.AddItem(product.Id, product.Name, product.Thumbnail, 100m, 1);
//         order.CalculateTotalPrice();
//         _orderRepositoryMock.GetByIdAsync(Arg.Is<OrderId>(id => id.Value == orderId), Arg.Any<CancellationToken>()).Returns(order);

//         var payment = Payment.Create(order.Id, PaymentProvider.Internal.ToString(), PaymentMethod.Balance.ToString(), order.TotalPrice);
//         payment.MarkAsPaid();
//         _paymentRepositoryMock.GetByOrderIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(payment);

//         var user = new UserFaker(_passwordHasherMock).Generate();
//         typeof(User).GetProperty(nameof(User.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(user, userId);
//         typeof(User).GetProperty(nameof(User.Balance), BindingFlags.Public | BindingFlags.Instance)!.SetValue(user, Money.Create(50m).Value);
//         _userRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

//         _productRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>())
//             .Returns(new List<Product> { product });

//         // Act
//         var result = await _handler.Handle(request, CancellationToken.None);

//         // Assert
//         result.IsSuccess.ShouldBeTrue();
//         user.Balance.Value.ShouldBe(150m);
//         order.Status.ShouldBe(OrderStatus.Refunded);
//         payment.Status.ShouldBe(PaymentStatus.Refunded);
//         await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
//     }

//     [Fact]
//     public async Task Handle_ShouldReturnFailure_WhenProviderRefundFails()
//     {
//         // Arrange
//         var orderId = Guid.NewGuid();
//         var request = new CancelOrderRequest { UserId = Guid.NewGuid(), OrderId = orderId, RefundToWallet = false };
//         var order = CreateOrder();
//         typeof(Order).GetProperty(nameof(Order.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(order, OrderId.Create(orderId));
//         order.CalculateTotalPrice();
//         _orderRepositoryMock.GetByIdAsync(Arg.Is<OrderId>(id => id.Value == orderId), Arg.Any<CancellationToken>()).Returns(order);

//         var payment = Payment.Create(order.Id, PaymentProvider.Paymob.ToString(), PaymentMethod.CreditCard.ToString(), order.TotalPrice);
//         payment.MarkAsPaid();
//         typeof(Payment).GetProperty(nameof(Payment.TransactionId), BindingFlags.Public | BindingFlags.Instance)!.SetValue(payment, "trans_123");
//         _paymentRepositoryMock.GetByOrderIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(payment);

//         var providerMock = _paymentFactoryMock.GetProvider(PaymentProvider.Paymob);
//         providerMock.RefundPaymentAsync(Arg.Any<string>(), Arg.Any<decimal>(), Arg.Any<CancellationToken>())
//             .Returns(Result.Failure(Error.Failure("Refund.Failed", "Failed")));

//         _productRepositoryMock.GetByIdsAsync(Arg.Any<List<ProductId>>(), Arg.Any<CancellationToken>())
//             .Returns(new List<Product>());

//         // Act
//         var result = await _handler.Handle(request, CancellationToken.None);

//         // Assert
//         result.IsFailure.ShouldBeTrue();
//         result.Error.Code.ShouldBe("Refund.Failed");
//     }

//     private Order CreateOrder(UserId? userId = null)
//     {
//         var address = Address.Create(1, "B", "S", "C", "1", "D").Value;
//         return Order.Create(userId ?? UserId.Create(Guid.NewGuid()), address);
//     }
// }
