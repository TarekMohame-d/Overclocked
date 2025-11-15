using Application.Abstraction.DomainServices;
using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;
using Microsoft.Extensions.Logging;

namespace Application.Services.Product.Decorators;

public class CachingProductServiceDecorator(
    IProductService inner,
    ICacheService cacheService,
    ILogger<CachingProductServiceDecorator> logger)
    : IProductService
{
    public Task<Result<PagedResult<ProductListResponse>>> GetPagedProductsAsync(
        GetPagedProductsRequest request, CancellationToken cancellationToken)
        => ExecuteWithCacheAsync(request, inner.GetPagedProductsAsync, cancellationToken);

    public Task<Result<ProductResponse>> GetProductByIdAsync(
        GetProductByIdRequest request, CancellationToken cancellationToken)
        => ExecuteWithCacheAsync(request, inner.GetProductByIdAsync, cancellationToken);

    public async Task<Result> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        Result result = await inner.CreateProductAsync(request, cancellationToken);

        if (result.IsSuccess)
            await cacheService.RemoveKeysInSetAsync(CacheKeys.ProductSet, cancellationToken);

        return result;
    }

    public async Task<Result> UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken)
    {
        Result result = await inner.UpdateProductAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            await cacheService.RemoveAsync(CacheKeys.Product(request.Id.ToString()), cancellationToken);
            await cacheService.RemoveKeysInSetAsync(CacheKeys.ProductSet, cancellationToken);
        }

        return result;
    }

    public async Task<Result> DeleteProductAsync(DeleteProductRequest request, CancellationToken cancellationToken)
    {
        Result result = await inner.DeleteProductAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            await cacheService.RemoveAsync(CacheKeys.Product(request.Id.ToString()), cancellationToken);
            await cacheService.RemoveKeysInSetAsync(CacheKeys.ProductSet, cancellationToken);
        }

        return result;
    }

    private async Task<Result<T>> ExecuteWithCacheAsync<T, TRequest>(
        TRequest request,
        Func<TRequest, CancellationToken, Task<Result<T>>> action,
        CancellationToken cancellationToken)
        where TRequest : ICachedRequest
    {
        var requestName = request.GetType().Name;
        var cacheKey = request.CacheKey;

        // Try get from cache
        Result<T>? cached = await cacheService.GetAsync<Result<T>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            logger.LogInformation("Cache hit for {RequestName}", requestName);
            return cached;
        }

        logger.LogInformation("Cache miss for {RequestName}", requestName);
        Result<T> result = await action(request, cancellationToken);

        if (result.IsSuccess)
        {
            await cacheService.SetAsync(cacheKey, result, request.SlidingExpiration, cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.CacheSetKey))
                await cacheService.AddToSetAsync(request.CacheSetKey, cacheKey);
        }

        return result;
    }
}
