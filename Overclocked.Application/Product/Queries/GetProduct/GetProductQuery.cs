using Overclocked.Application.Abstraction.Messaging;
using Overclocked.Application.Common.Constants;

namespace Overclocked.Application.Product.Queries.GetProduct;

public record GetProductQuery : ICachedQuery
{
    public required Guid Id { get; init; }
    public string CacheKey => CacheKeys.Product(Id.ToString());
    public string? CacheSetKey => null;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}
