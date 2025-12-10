using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.BrandAggregate.Events;

public record BrandUpdatedEvent(Guid BrandId, string ImageUrl) : IDomainEvent;
