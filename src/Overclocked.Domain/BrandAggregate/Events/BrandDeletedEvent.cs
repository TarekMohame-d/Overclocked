using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.BrandAggregate.Events;

public record BrandDeletedEvent(Guid BrandId, string ImageUrl) : IDomainEvent;
