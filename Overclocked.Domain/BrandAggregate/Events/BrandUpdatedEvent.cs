using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.BrandAggregate.Events;

public record BrandUpdatedEvent(string ImageUrl) : IDomainEvent;
