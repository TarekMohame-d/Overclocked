using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstraction.Messaging;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Application.Brand.Queries.GetAllBrands;
using Overclocked.Application.Brand.Queries.GetBrand;
using Overclocked.Contracts.Brand;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Brand.Queries.Decorators;

public class CachingBrandQueriesDecorator(
    IBrandQueries inner,
    ICacheService cacheService,
    ILogger<CachingBrandQueriesDecorator> logger) : IBrandQueries
{
    public Task<Result<BrandResponse>> GetBrandQueryHandler(
        GetBrandQuery query,
        CancellationToken cancellationToken) =>
            ExecuteWithCacheAsync(query, inner.GetBrandQueryHandler, cancellationToken);

    public Task<Result<IEnumerable<BrandListResponse>>> GetBrandListQueryHandler(
        GetBrandListQuery query,
        CancellationToken cancellationToken) =>
            ExecuteWithCacheAsync(query, inner.GetBrandListQueryHandler, cancellationToken);

    private async Task<Result<T>> ExecuteWithCacheAsync<T, TRequest>(
        TRequest query,
        Func<TRequest, CancellationToken, Task<Result<T>>> action,
        CancellationToken cancellationToken)
        where TRequest : ICachedQuery
    {
        var queryName = query.GetType().Name;
        var cacheKey = query.CacheKey;

        // Try get from cache
        T? cachedValue = await cacheService.GetAsync<T>(cacheKey, cancellationToken);
        if(cachedValue is not null)
        {
            logger.LogInformation("Cache hit for {QueryName}", queryName);
            return Result<T>.Success(cachedValue);
        }

        logger.LogInformation("Cache miss for {QueryName}", queryName);
        Result<T> result = await action(query, cancellationToken);

        if(result.IsSuccess)
        {
            await cacheService.SetAsync(cacheKey, result.Value, query.SlidingExpiration, cancellationToken);
        }

        return result;
    }
}
