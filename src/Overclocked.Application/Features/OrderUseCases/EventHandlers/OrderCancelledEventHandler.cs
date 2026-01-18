using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.OrderAggregate.Events;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Application.Features.OrderUseCases.EventHandlers;

public class OrderCancelledEventHandler(
    IOrderReadRepository orderRepository,
    IUserReadRepository userRepository,
    IEmailService emailService
) : IDomainEventHandler<OrderCancelledEvent>
{
    public async Task Handle(OrderCancelledEvent domainEvent, CancellationToken ct = default)
    {
        Order? order = await orderRepository.GetByIdAsync(OrderId.Create(domainEvent.OrderId), ct);
        User? user = await userRepository.GetByIdAsync(order!.UserId, ct);

        await emailService.SendOrderCancellationEmail(user!.Email, order.OrderNumber);
    }
}
