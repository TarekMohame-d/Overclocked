using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.CategoryAggregate.Events;

public record CategoryDeletedEvent(Guid CategoryId, string ImageUrl) : IDomainEvent;
