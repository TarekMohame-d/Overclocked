using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.ReviewAggregate.Events;

public record ReviewUpdatedEvent(Guid ProductId, int OldRating, int NewRating) : IDomainEvent;
