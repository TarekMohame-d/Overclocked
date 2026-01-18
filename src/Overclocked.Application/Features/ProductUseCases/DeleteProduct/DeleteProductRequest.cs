using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.ProductUseCases.DeleteProduct;

public record DeleteProductRequest : IRequest, ICacheInvalidatorRequest
{
    public required Guid Id { get; init; }

    public string[] CacheKeys => [Common.Constants.CacheKeys.Product(Id.ToString())];
    public string? CacheSetKey => Common.Constants.CacheKeys.ProductSet;
}
