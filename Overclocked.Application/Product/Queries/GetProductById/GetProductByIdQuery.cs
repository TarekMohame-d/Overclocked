using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Contracts.Product;

namespace Overclocked.Application.Product.Queries.GetProductById;

public record GetProductByIdQuery : IQuery<ProductResponse>, ICachedQuery
{
    public required Guid Id { get; init; }
    public string CacheKey => CacheKeys.Product(Id.ToString());
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
