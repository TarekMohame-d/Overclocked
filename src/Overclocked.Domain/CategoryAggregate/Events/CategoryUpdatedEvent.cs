using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.CategoryAggregate.Events;

public record CategoryImageUpdatedEvent(Guid CategoryId, string ImageUrl) : IDomainEvent;
