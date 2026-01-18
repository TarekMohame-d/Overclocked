// using System.Reflection;
// using NSubstitute;
// using Overclocked.Application.Abstractions.Persistence;
// using Overclocked.Application.Abstractions.Services;
// using Overclocked.Application.Features.OrderUseCases.EventHandlers;
// using Overclocked.Architecture.Tests.FakeData;
// using Overclocked.Domain.Common.Shared.ValueObjects.Address;
// using Overclocked.Domain.OrderAggregate;
// using Overclocked.Domain.OrderAggregate.Events;
// using Overclocked.Domain.OrderAggregate.ValueObjects;
// using Overclocked.Domain.PaymentAggregate.Events;
// using Overclocked.Domain.UserAggregate;
// using Overclocked.Domain.UserAggregate.ValueObjects;
// using Shouldly;

// namespace Overclocked.Unit.Tests.OrderTests;

// public class OrderEventHandlersTests
// {
//     private readonly IOrderReadRepository _orderRepositoryMock;
//     private readonly IUserReadRepository _userRepositoryMock;
//     private readonly IEmailService _emailServiceMock;
//     private readonly IPasswordHasher _passwordHasherMock;

//     public OrderEventHandlersTests()
//     {
//         _orderRepositoryMock = Substitute.For<IOrderReadRepository>();
//         _userRepositoryMock = Substitute.For<IUserReadRepository>();
//         _emailServiceMock = Substitute.For<IEmailService>();
//         _passwordHasherMock = Substitute.For<IPasswordHasher>();
//     }

//     [Fact]
//     public async Task OrderCancelledEventHandler_Should_SendEmail()
//     {
//         // Arrange
//         var userId = UserId.Create(Guid.NewGuid());
//         var order = CreateOrder(userId);
//         var user = new UserFaker(_passwordHasherMock).Generate();
//         typeof(User).GetProperty(nameof(User.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(user, userId);

//         _orderRepositoryMock.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
//         _userRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

//         var handler = new OrderCancelledEventHandler(_orderRepositoryMock, _userRepositoryMock, _emailServiceMock);
//         var domainEvent = new OrderCancelledEvent(order.Id.Value);

//         // Act
//         await handler.Handle(domainEvent, CancellationToken.None);

//         // Assert
//         await _emailServiceMock.Received(1).SendOrderCancellationEmail(user.Email, order.OrderNumber);
//     }

//     [Fact]
//     public async Task OrderPlacedEventHandler_Should_SendEmail()
//     {
//         // Arrange
//         var userId = UserId.Create(Guid.NewGuid());
//         var order = CreateOrder(userId);
//         order.CalculateTotalPrice();
//         var user = new UserFaker(_passwordHasherMock).Generate();
//         typeof(User).GetProperty(nameof(User.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(user, userId);

//         _orderRepositoryMock.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
//         _userRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

//         var handler = new OrderPlacedEventHandler(_orderRepositoryMock, _userRepositoryMock, _emailServiceMock);
//         var domainEvent = new OrderPlacedEvent(order.Id.Value, IsCod: true, IsBalance: false);

//         // Act
//         await handler.Handle(domainEvent, CancellationToken.None);

//         // Assert
//         await _emailServiceMock.Received(1).SendOrderConfirmationEmail(
//             user.Email,
//             order.OrderNumber,
//             order.TotalPrice.Value.ToString(),
//             true,
//             false
//         );
//     }

//     [Fact]
//     public async Task OrderRefundedEventHandler_Should_SendEmail()
//     {
//         // Arrange
//         var userId = UserId.Create(Guid.NewGuid());
//         var order = CreateOrder(userId);
//         order.CalculateTotalPrice();
//         var user = new UserFaker(_passwordHasherMock).Generate();
//         typeof(User).GetProperty(nameof(User.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(user, userId);

//         _orderRepositoryMock.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
//         _userRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

//         var handler = new OrderRefundedEventHandler(_orderRepositoryMock, _userRepositoryMock, _emailServiceMock);
//         var domainEvent = new OrderRefundedEvent(order.Id.Value, true);

//         // Act
//         await handler.Handle(domainEvent, CancellationToken.None);

//         // Assert
//         await _emailServiceMock.Received(1).SendOrderRefundedEmail(
//             user.Email,
//             order.OrderNumber,
//             order.TotalPrice.Value.ToString(),
//             true
//         );
//     }

//     [Fact]
//     public async Task PaymentFailedEventHandler_Should_SendEmail()
//     {
//         // Arrange
//         var userId = UserId.Create(Guid.NewGuid());
//         var order = CreateOrder(userId);
//         order.CalculateTotalPrice();
//         var user = new UserFaker(_passwordHasherMock).Generate();
//         typeof(User).GetProperty(nameof(User.Id), BindingFlags.Public | BindingFlags.Instance)!.SetValue(user, userId);

//         _orderRepositoryMock.GetByIdAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
//         _userRepositoryMock.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

//         var handler = new PaymentFailedEventHandler(_orderRepositoryMock, _userRepositoryMock, _emailServiceMock);
//         var domainEvent = new PaymentFailedEvent(order.Id.Value);

//         // Act
//         await handler.Handle(domainEvent, CancellationToken.None);

//         // Assert
//         await _emailServiceMock.Received(1).SendPaymentFailedEmail(
//             user.Email,
//             order.OrderNumber,
//             order.TotalPrice.Value.ToString()
//         );
//     }

//     private Order CreateOrder(UserId? userId = null)
//     {
//         var address = Address.Create(1, "B", "S", "C", "1", "D").Value;
//         return Order.Create(userId ?? UserId.Create(Guid.NewGuid()), address);
//     }
// }
