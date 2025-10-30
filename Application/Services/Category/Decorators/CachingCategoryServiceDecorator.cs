using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.DTOs.Response;
using Microsoft.Extensions.Logging;

namespace Application.Services.Category.Decorators;

public class CachingCategoryServiceDecorator : ICategoryService
{
    private readonly ICategoryService _inner;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingCategoryServiceDecorator> _logger;

    public CachingCategoryServiceDecorator(
        ICategoryService inner,
        ICacheService cacheService,
        ILogger<CachingCategoryServiceDecorator> logger)
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

    public Task<Result<IEnumerable<CategoryListResponse>>> GetAllCategoriesAsync(
    GetAllCategoriesRequest request, CancellationToken cancellationToken)
    => ExecuteWithCacheAsync(request, _inner.GetAllCategoriesAsync, cancellationToken);

    public Task<Result<CategoryResponse>> GetCategoryByIdAsync(
        GetCategoryByIdRequest request, CancellationToken cancellationToken)
        => ExecuteWithCacheAsync(request, _inner.GetCategoryByIdAsync, cancellationToken);

    public async Task<Result> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _inner.CreateCategoryAsync(request, cancellationToken);

        if (result.IsSuccess)
            await _cacheService.RemoveAsync(CacheKeys.AllCategories, cancellationToken);

        return result;
    }

    public async Task<Result> UpdateCategoryAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _inner.UpdateCategoryAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            await _cacheService.RemoveAsync(CacheKeys.Category(request.Id.ToString()), cancellationToken);
            await _cacheService.RemoveAsync(CacheKeys.AllCategories, cancellationToken);
        }
        return result;
    }

    public async Task<Result> DeleteCategoryAsync(DeleteCategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _inner.DeleteCategoryAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            await _cacheService.RemoveAsync(CacheKeys.Category(request.Id.ToString()), cancellationToken);
            await _cacheService.RemoveAsync(CacheKeys.AllCategories, cancellationToken);
        }

        return result;
    }
}
