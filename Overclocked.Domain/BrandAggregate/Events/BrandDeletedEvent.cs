using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.BrandAggregate.Events;

public record BrandDeletedEvent(Guid BrandId, string ImageUrl) : IDomainEvent;
