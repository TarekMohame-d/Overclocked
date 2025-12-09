using Overclocked.Application.Abstraction.Messaging;
using Overclocked.Application.Common.Constants;

namespace Overclocked.Application.Brand.Queries.GetAllBrands;

public record GetBrandListQuery : ICachedQuery
{
    public string CacheKey => CacheKeys.AllBrands;
    public string? CacheSetKey => null;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}
