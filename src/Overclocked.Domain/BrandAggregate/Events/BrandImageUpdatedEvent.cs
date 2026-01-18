using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.BrandAggregate.Events;

public record BrandImageUpdatedEvent(Guid BrandId, string ImageUrl) : IDomainEvent;
