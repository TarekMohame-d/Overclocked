using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.PaymentAggregate.Events
{
    public record PaymentFailedEvent(Guid OrderId) : IDomainEvent;
}
