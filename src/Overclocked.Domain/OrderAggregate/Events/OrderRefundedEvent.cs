using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.OrderAggregate.Events;

public record OrderRefundedEvent(Guid OrderId, bool AddToBalance) : IDomainEvent;
