using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.BrandAggregate.Events;

public record BrandDeletedEvent(string ImageUrl) : IDomainEvent;
