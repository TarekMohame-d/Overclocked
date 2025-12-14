using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Contracts.Brand;

namespace Overclocked.Application.Brand.Queries.GetAllBrands;

public record GetAllBrandsQuery : IQuery<IEnumerable<BrandListResponse>>, ICachedQuery
{
    public string CacheKey => CacheKeys.AllBrands;
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
