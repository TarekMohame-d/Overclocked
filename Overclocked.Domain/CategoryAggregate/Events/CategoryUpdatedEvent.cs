using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.CategoryAggregate.Events;

public record CategoryUpdatedEvent(string ImageUrl) : IDomainEvent;
