using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;
using Microsoft.Extensions.Logging;

namespace Application.Services.Product.Decorators;

public class CachingProductServiceDecorator : IProductService
{
    private readonly IProductService _inner;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingProductServiceDecorator> _logger;

    public CachingProductServiceDecorator(
        IProductService inner,
        ICacheService cacheService,
        ILogger<CachingProductServiceDecorator> logger)
    {
        _inner = inner;
        _cacheService = cacheService;
        _logger = logger;
    }

    private async Task<Result<T>> ExecuteWithCacheAsync<T, TRequest>(
    TRequest request,
    Func<TRequest, CancellationToken, Task<Result<T>>> action,
    CancellationToken cancellationToken)
    where TRequest : ICachedRequest
    {
        string requestName = request.GetType().Name;
        string cacheKey = request.CacheKey;

        // Try get from cache
        var cached = await _cacheService.GetAsync<Result<T>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _logger.LogInformation("Cache hit for {RequestName}", requestName);
            return cached;
        }

        _logger.LogInformation("Cache miss for {RequestName}", requestName);
        var result = await action(request, cancellationToken);

        if (result.IsSuccess)
        {
            await _cacheService.SetAsync(cacheKey, result, request.SlidingExpiration, cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.CacheSetKey))
            {
                await _cacheService.AddToSetAsync(request.CacheSetKey, cacheKey);
            }
        }

        return result;
    }

    public Task<Result<PagedResult<ProductListResponse>>> GetPagedProductsAsync(
    GetPagedProductsQuery query, CancellationToken cancellationToken)
    => ExecuteWithCacheAsync(query, _inner.GetPagedProductsAsync, cancellationToken);

    public Task<Result<ProductResponse>> GetProductByIdAsync(
        GetProductByIdRequest request, CancellationToken cancellationToken)
        => ExecuteWithCacheAsync(request, _inner.GetProductByIdAsync, cancellationToken);

    public async Task<Result> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _inner.CreateProductAsync(request, cancellationToken);

        if (result.IsSuccess)
            await _cacheService.RemoveKeysInSetAsync(CacheKeys.ProductSet, cancellationToken);

        return result;
    }

    public async Task<Result> UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _inner.UpdateProductAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            await _cacheService.RemoveAsync(CacheKeys.Product(request.Id.ToString()), cancellationToken);
            await _cacheService.RemoveKeysInSetAsync(CacheKeys.ProductSet, cancellationToken);
        }
        return result;
    }

    public async Task<Result> DeleteProductAsync(DeleteProductRequest request, CancellationToken cancellationToken)
    {
        var result = await _inner.DeleteProductAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            await _cacheService.RemoveAsync(CacheKeys.Product(request.Id.ToString()), cancellationToken);
            await _cacheService.RemoveKeysInSetAsync(CacheKeys.ProductSet, cancellationToken);
        }

        return result;
    }
}
