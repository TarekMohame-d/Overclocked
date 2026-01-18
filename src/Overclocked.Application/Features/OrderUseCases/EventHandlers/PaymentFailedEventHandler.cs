using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.OrderAggregate;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.PaymentAggregate.Events;
using Overclocked.Domain.UserAggregate;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Application.Features.OrderUseCases.EventHandlers
{
    public class PaymentFailedEventHandler(
        IOrderReadRepository orderRepository,
        IUserReadRepository userRepository,
        IEmailService emailService
    ) : IDomainEventHandler<PaymentFailedEvent>
    {
        public async Task Handle(PaymentFailedEvent domainEvent, CancellationToken ct = default)
        {
            Order? order = await orderRepository.GetByIdAsync(OrderId.Create(domainEvent.OrderId), ct);
            User? user = await userRepository.GetByIdAsync(order!.UserId, ct);

            var orderTotal = order.TotalPrice.Value.ToString();
            var orderNumber = order.OrderNumber;

            await emailService.SendPaymentFailedEmail(user!.Email, orderNumber, orderTotal);
        }
    }
}
