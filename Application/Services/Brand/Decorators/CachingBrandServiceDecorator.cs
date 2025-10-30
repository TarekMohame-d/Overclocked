using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.DTOs.Response;
using Application.Services.Tag.DTOs.Request;
using Microsoft.Extensions.Logging;

namespace Application.Services.Brand.Decorators;

public class CachingBrandServiceDecorator : IBrandService
{
    private readonly IBrandService _inner;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingBrandServiceDecorator> _logger;

    public CachingBrandServiceDecorator(
        IBrandService inner,
        ICacheService cacheService,
        ILogger<CachingBrandServiceDecorator> logger)
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

    public Task<Result<IEnumerable<BrandListResponse>>> GetAllBrandsAsync(
    GetAllBrandsRequest request, CancellationToken cancellationToken)
    => ExecuteWithCacheAsync(request, _inner.GetAllBrandsAsync, cancellationToken);

    public Task<Result<BrandResponse>> GetBrandByIdAsync(
        GetBrandByIdRequest request, CancellationToken cancellationToken)
        => ExecuteWithCacheAsync(request, _inner.GetBrandByIdAsync, cancellationToken);

    public async Task<Result> CreateBrandAsync(CreateBrandRequest request, CancellationToken cancellationToken)
    {
        var result = await _inner.CreateBrandAsync(request, cancellationToken);

        if (result.IsSuccess)
            await _cacheService.RemoveAsync(CacheKeys.AllBrands, cancellationToken);

        return result;
    }

    public async Task<Result> UpdateBrandAsync(UpdateBrandRequest request, CancellationToken cancellationToken)
    {
        var result = await _inner.UpdateBrandAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            await _cacheService.RemoveAsync(CacheKeys.Brand(request.Id.ToString()), cancellationToken);
            await _cacheService.RemoveAsync(CacheKeys.AllBrands, cancellationToken);
        }
        return result;
    }

    public async Task<Result> DeleteBrandAsync(DeleteBrandRequest request, CancellationToken cancellationToken)
    {
        var result = await _inner.DeleteBrandAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            await _cacheService.RemoveAsync(CacheKeys.Brand(request.Id.ToString()), cancellationToken);
            await _cacheService.RemoveAsync(CacheKeys.AllBrands, cancellationToken);
        }

        return result;
    }
}
