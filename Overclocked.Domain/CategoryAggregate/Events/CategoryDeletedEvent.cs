using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.CategoryAggregate.Events;

public record CategoryDeletedEvent(string ImageUrl) : IDomainEvent;
