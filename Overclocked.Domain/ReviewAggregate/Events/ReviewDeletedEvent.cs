using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.ReviewAggregate.Events;

public record ReviewDeletedEvent(Guid ProductId, int Rating) : IDomainEvent;
