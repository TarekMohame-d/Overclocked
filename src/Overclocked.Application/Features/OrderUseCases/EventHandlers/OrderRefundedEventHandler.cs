using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.OrderAggregate.Events;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Application.Features.OrderUseCases.EventHandlers;

public class OrderRefundedEventHandler(
    IOrderReadRepository orderRepository,
    IUserReadRepository userRepository,
    IEmailService emailService
) : IDomainEventHandler<OrderRefundedEvent>
{
    public async Task Handle(OrderRefundedEvent domainEvent, CancellationToken ct = default)
    {
        Order? order = await orderRepository.GetByIdAsync(OrderId.Create(domainEvent.OrderId), ct);
        User? user = await userRepository.GetByIdAsync(order!.UserId, ct);

        var orderTotal = order.TotalPrice.Value.ToString();
        var orderNumber = order.OrderNumber;

        await emailService.SendOrderRefundedEmail(user!.Email, orderNumber, orderTotal, domainEvent.AddToBalance);
    }
}
