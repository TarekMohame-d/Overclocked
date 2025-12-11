using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstraction.Messaging;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Application.Product.Queries.GetPagedProducts;
using Overclocked.Application.Product.Queries.GetProduct;
using Overclocked.Contracts.Product;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Product.Queries.Decorators;

public class CachingProductQueriesDecorator(
    IProductQueries inner,
    ICacheService cacheService,
    ILogger<CachingProductQueriesDecorator> logger) : IProductQueries
{
    public Task<Result<ProductResponse>> GetProductQueryHandler(
        GetProductQuery query,
        CancellationToken cancellationToken) =>
            ExecuteWithCacheAsync(query, inner.GetProductQueryHandler, cancellationToken);

    public Task<Result<PagedResult<ProductPagedResponse>>> GetPagedProductsQueryHandler(
        GetPagedProductsQuery query,
        CancellationToken cancellationToken) =>
            ExecuteWithCacheAsync(query, inner.GetPagedProductsQueryHandler, cancellationToken);

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

            if(!string.IsNullOrWhiteSpace(query.CacheSetKey))
            {
                await cacheService.AddToSetAsync(query.CacheSetKey, cacheKey);
            }
        }

        return result;
    }
}
