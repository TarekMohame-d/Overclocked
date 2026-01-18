using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.OrderAggregate.Events;

public record OrderCancelledEvent(Guid OrderId) : IDomainEvent;
