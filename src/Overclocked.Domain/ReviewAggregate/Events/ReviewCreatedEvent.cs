using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.ReviewAggregate.Events;

public record ReviewCreatedEvent(Guid ProductId, int Rating) : IDomainEvent;
