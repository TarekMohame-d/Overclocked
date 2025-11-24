using Application.Abstraction.DomainServices;
using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Category.DTOs.Request;
using Application.Services.Category.DTOs.Response;
using Microsoft.Extensions.Logging;

namespace Application.Services.Category.Decorators;

public class CachingCategoryServiceDecorator(
    ICategoryService inner,
    ICacheService cacheService,
    ILogger<CachingCategoryServiceDecorator> logger)
    : ICategoryService
{
    public Task<Result<IEnumerable<CategoryListResponse>>> GetAllCategoriesAsync(
        GetAllCategoriesRequest request,
        CancellationToken cancellationToken) =>
            ExecuteWithCacheAsync(request, inner.GetAllCategoriesAsync, cancellationToken);

    public Task<Result<CategoryResponse>> GetCategoryByIdAsync(
        GetCategoryByIdRequest request,
        CancellationToken cancellationToken) =>
            ExecuteWithCacheAsync(request, inner.GetCategoryByIdAsync, cancellationToken);

    public async Task<Result> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        Result result = await inner.CreateCategoryAsync(request, cancellationToken);

        if(result.IsSuccess)
        {
            await cacheService.RemoveAsync(CacheKeys.AllCategories, cancellationToken);
        }

        return result;
    }

    public async Task<Result> UpdateCategoryAsync(UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        Result result = await inner.UpdateCategoryAsync(request, cancellationToken);

        if(result.IsSuccess)
        {
            await cacheService.RemoveAsync(CacheKeys.Category(request.Id.ToString()), cancellationToken);
            await cacheService.RemoveAsync(CacheKeys.AllCategories, cancellationToken);
        }

        return result;
    }

    public async Task<Result> DeleteCategoryAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        Result result = await inner.DeleteCategoryAsync(categoryId, cancellationToken);

        if(result.IsSuccess)
        {
            await cacheService.RemoveAsync(CacheKeys.Category(categoryId.ToString()), cancellationToken);
            await cacheService.RemoveAsync(CacheKeys.AllCategories, cancellationToken);
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
