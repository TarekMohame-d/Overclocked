using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.ProductAggregate.Events;

public record ProductImagesRemovedEvent(Guid ProductId, IEnumerable<string> ImagesUrls) : IDomainEvent;
