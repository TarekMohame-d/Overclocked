using Application.Abstraction.DomainServices;
using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.DTOs.Response;
using Microsoft.Extensions.Logging;

namespace Application.Services.Brand.Decorators;

public class CachingBrandServiceDecorator(
    IBrandService inner,
    ICacheService cacheService,
    ILogger<CachingBrandServiceDecorator> logger)
    : IBrandService
{
    public Task<Result<IEnumerable<BrandListResponse>>> GetAllBrandsAsync(
        GetAllBrandsRequest request,
        CancellationToken cancellationToken) =>
            ExecuteWithCacheAsync(request, inner.GetAllBrandsAsync, cancellationToken);

    public Task<Result<BrandResponse>> GetBrandByIdAsync(
        GetBrandByIdRequest request,
        CancellationToken cancellationToken) =>
            ExecuteWithCacheAsync(request, inner.GetBrandByIdAsync, cancellationToken);

    public async Task<Result> CreateBrandAsync(CreateBrandRequest request, CancellationToken cancellationToken)
    {
        Result result = await inner.CreateBrandAsync(request, cancellationToken);

        if(result.IsSuccess)
            await cacheService.RemoveAsync(CacheKeys.AllBrands, cancellationToken);

        return result;
    }

    public async Task<Result> UpdateBrandAsync(UpdateBrandRequest request, CancellationToken cancellationToken)
    {
        Result result = await inner.UpdateBrandAsync(request, cancellationToken);

        if(result.IsSuccess)
        {
            await cacheService.RemoveAsync(CacheKeys.Brand(request.Id.ToString()), cancellationToken);
            await cacheService.RemoveAsync(CacheKeys.AllBrands, cancellationToken);
        }

        return result;
    }

    public async Task<Result> DeleteBrandAsync(Guid brandId, CancellationToken cancellationToken)
    {
        Result result = await inner.DeleteBrandAsync(brandId, cancellationToken);

        if(result.IsSuccess)
        {
            await cacheService.RemoveAsync(CacheKeys.Brand(brandId.ToString()), cancellationToken);
            await cacheService.RemoveAsync(CacheKeys.AllBrands, cancellationToken);
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
        if(cached is not null)
        {
            logger.LogInformation("Cache hit for {RequestName}", requestName);
            return cached;
        }

        logger.LogInformation("Cache miss for {RequestName}", requestName);
        Result<T> result = await action(request, cancellationToken);

        if(result.IsSuccess)
        {
            await cacheService.SetAsync(cacheKey, result, request.SlidingExpiration, cancellationToken);

            if(!string.IsNullOrWhiteSpace(request.CacheSetKey))
                await cacheService.AddToSetAsync(request.CacheSetKey, cacheKey);
        }

        return result;
    }
}
