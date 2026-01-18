using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.OrderAggregate.Events;

public record OrderPlacedEvent(Guid OrderId, bool IsCod, bool IsBalance) : IDomainEvent;
