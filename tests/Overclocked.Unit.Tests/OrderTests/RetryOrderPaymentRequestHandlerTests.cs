// using System.Reflection;
// using NSubstitute;
// using Overclocked.Application.Abstractions;
// using Overclocked.Application.Abstractions.Factories;
// using Overclocked.Application.Abstractions.Persistence;
// using Overclocked.Application.Abstractions.Services;
// using Overclocked.Application.Common.Constants.Payment;
// using Overclocked.Application.Features.OrderUseCases.DTOs.Requests;
// using Overclocked.Application.Features.OrderUseCases.RetryOrderPayment;
// using Overclocked.Architecture.Tests.FakeData;
// using Overclocked.Domain.Common.Shared.ValueObjects.Address;
// using Overclocked.Domain.Common.Shared.ValueObjects.Money;
// using Overclocked.Domain.OrderAggregate;
// using Overclocked.Domain.OrderAggregate.ValueObjects;
// using Overclocked.Domain.PaymentAggregate;
// using Overclocked.Domain.PaymentAggregate.ValueObjects;
// using Overclocked.Domain.ProductAggregate.ValueObjects;
// using Overclocked.Domain.UserAggregate;
// using Overclocked.Domain.UserAggregate.ValueObjects;
// using Overclocked.SharedKernel;
// using Shouldly;

// namespace Overclocked.Unit.Tests.OrderTests;

// public class RetryOrderPaymentRequestHandlerTests
// {
//     private readonly IOrderRepository _orderRepositoryMock;
//     private readonly IUserRepository _userRepositoryMock;
//     private readonly IPaymentRepository _paymentRepositoryMock;
//     private readonly IUnitOfWork _unitOfWorkMock;
//     private readonly PaymentFactory _paymentFactoryMock;
//     private readonly IPasswordHasher _passwordHasherMock;
//     private readonly RetryOrderPaymentRequestHandler _handler;

//     public RetryOrderPaymentRequestHandlerTests()
//     {
//         _orderRepositoryMock = Substitute.For<IOrderRepository>();
//         _userRepositoryMock = Substitute.For<IUserRepository>();
//         _paymentRepositoryMock = Substitute.For<IPaymentRepository>();
//         _unitOfWorkMock = Substitute.For<IUnitOfWork>();
//         _passwordHasherMock = Substitute.For<IPasswordHasher>();

//         var stripeProvider = Substitute.For<IPaymentProviderService>();
//         stripeProvider.PaymentProvider.Returns(PaymentProvider.Paymob);
//         var walletProvider = Substitute.For<IPaymentProviderService>();
//         walletProvider.PaymentProvider.Returns(PaymentProvider.Internal);

//         _paymentFactoryMock = new PaymentFactory(new List<IPaymentProviderService> { stripeProvider, walletProvider });

//         _handler = new RetryOrderPaymentRequestHandler(
//             _orderRepositoryMock,
//             _userRepositoryMock,
//             _paymentRepositoryMock,
//             _unitOfWorkMock,
//             _paymentFactoryMock
//         );
//     }

//     [Fact]
//     public async Task Handle_ShouldReturnFailure_WhenAddressIsInvalid()
//     {
//         // Arrange
//         var request = CreateRequest(apartment: -1);

//         // Act
//         var result = await _handler.Handle(request, CancellationToken.None);

//         // Assert
//         result.IsFailure.ShouldBeTrue();
//         result.Error.Code.ShouldBe("Address.Apartment");
//     }

//     [Fact]
//     public async Task Handle_ShouldReturnFailure_WhenOrderNotFound()
//     {
//         // Arrange
//         var request = CreateRequest();
//         _orderRepositoryMock.GetByIdAsync(Arg.Any<OrderId>(), Arg.Any<CancellationToken>()).Returns((Order)null!);

//         // Act
//         var result = await _handler.Handle(request, CancellationToken.None);

//         // Assert
//         result.IsFailure.ShouldBeTrue();
//         result.Error.Code.ShouldContain("Order.NotFound");
//     }

//     [Fact]
//     public async Task Handle_ShouldReturnFailure_WhenOrderNotPendingPayment()
//     {
//         // Arrange
//         var userId = UserId.Create(Guid.NewGuid());
//         var request = CreateRequest(userId: userId.Value);
//         var order = CreateOrder(userId);
//         order.MarkAsPlaced();
//         _orderRepositoryMock.GetByIdAsync(Arg.Any<OrderId>(), Arg.Any<CancellationToken>()).Returns(order);

//         // Act
//         var result = await _handler.Handle(request, CancellationToken.None);

//         // Assert
//         result.IsFailure.ShouldBeTrue();
//         result.Error.ShouldBe(OrderErrors.NotInPendingPaymentState);
//     }

//     [Fact]
//     public async Task Handle_ShouldSucceed_WhenRetryingWithBalance_AndSufficientFunds()
//     {
//         // Arrange
//         var userId = UserId.Create(Guid.NewGuid());
//         var request = CreateRequest(userId: userId.Value);
//         var order = CreateOrder(userId);
//         order.AddItem(ProductId.Create(Guid.NewGuid()), "P", null!, 100m, 1);
//         order.CalculateTotalPrice();
//         _orderRepositoryMock.GetByIdAsync(Arg.Any<OrderId>(), Arg.Any<CancellationToken>()).Returns(order);

//         var payment = Payment.Create(order.Id, PaymentProvider.Paymob.ToString(), PaymentMethod.CreditCard.ToString(), order.TotalPrice);
//         _paymentRepositoryMock.GetByOrderIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(payment);

//         var user = new UserFaker(_passwordHasherMock).Generate();
//         typeof(User).GetProperty(nameof(User.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(user, userId);
//         typeof(User).GetProperty(nameof(User.Balance), BindingFlags.Public | BindingFlags.Instance)!.SetValue(user, Money.Create(200m).Value);
//         _userRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

//         // Act
//         var result = await _handler.Handle(request, CancellationToken.None);

//         // Assert
//         result.IsSuccess.ShouldBeTrue();
//         user.Balance.Value.ShouldBe(100m);
//         order.Status.ShouldBe(OrderStatus.Placed);
//         payment.Status.ShouldBe(PaymentStatus.Paid);
//         payment.PaymentMethod.ShouldBe(PaymentMethod.Balance.ToString());
//         await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
//     }

//     private RetryOrderPaymentRequest CreateRequest(Guid? userId = null, int apartment = 1)
//     {
//         return new RetryOrderPaymentRequest
//         {
//             UserId = userId ?? Guid.NewGuid(),
//             OrderId = Guid.NewGuid(),
//             ShippingAddress = new ShippingAddressRequestDto(
//                 apartment,
//                 "Building",
//                 "Street",
//                 "City",
//                 "12345",
//                 "Description"
//             ),
//             PaymentMethod = PaymentMethod.Balance,
//             PaymentProvider = PaymentProvider.Internal
//         };
//     }

//     private Order CreateOrder(UserId? userId = null)
//     {
//         var address = Address.Create(1, "B", "S", "C", "1", "D").Value;
//         return Order.Create(userId ?? UserId.Create(Guid.NewGuid()), address);
//     }
// }
