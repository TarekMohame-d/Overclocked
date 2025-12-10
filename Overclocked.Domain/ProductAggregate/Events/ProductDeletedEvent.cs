using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.ProductAggregate.Events;

public record ProductDeletedEvent(Guid ProductId, IEnumerable<string> ImagesUrls) : IDomainEvent;
