using Application.Abstraction.DomainServices;
using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.DTOs.Response;
using Microsoft.Extensions.Logging;

namespace Application.Services.Tag.Decorators;

public class CachingTagServiceDecorator(
    ITagService inner,
    ICacheService cacheService,
    ILogger<CachingTagServiceDecorator> logger
) : ITagService
{
    public Task<Result<PagedResult<TagListResponse>>> GetPagedTagsAsync(
        GetPagedTagsRequest request,
        CancellationToken cancellationToken
    ) => ExecuteWithCacheAsync(request, inner.GetPagedTagsAsync, cancellationToken);

    public Task<Result<TagResponse>> GetTagByIdAsync(GetTagByIdRequest request, CancellationToken cancellationToken) =>
        ExecuteWithCacheAsync(request, inner.GetTagByIdAsync, cancellationToken);

    public async Task<Result> CreateTagAsync(CreateTagRequest request, CancellationToken cancellationToken)
    {
        Result result = await inner.CreateTagAsync(request, cancellationToken);

        if(result.IsSuccess)
        {
            await cacheService.RemoveKeysInSetAsync(CacheKeys.TagSet, cancellationToken);
        }

        return result;
    }

    public async Task<Result> UpdateTagAsync(UpdateTagRequest request, CancellationToken cancellationToken)
    {
        Result result = await inner.UpdateTagAsync(request, cancellationToken);

        if(result.IsSuccess)
        {
            await cacheService.RemoveAsync(CacheKeys.Tag(request.Id.ToString()), cancellationToken);
            await cacheService.RemoveKeysInSetAsync(CacheKeys.TagSet, cancellationToken);
        }

        return result;
    }

    public async Task<Result> DeleteTagAsync(DeleteTagRequest request, CancellationToken cancellationToken)
    {
        Result result = await inner.DeleteTagAsync(request, cancellationToken);

        if(result.IsSuccess)
        {
            await cacheService.RemoveAsync(CacheKeys.Tag(request.Id.ToString()), cancellationToken);
            await cacheService.RemoveKeysInSetAsync(CacheKeys.TagSet, cancellationToken);
        }

        return result;
    }

    private async Task<Result<T>> ExecuteWithCacheAsync<T, TRequest>(
        TRequest request,
        Func<TRequest, CancellationToken, Task<Result<T>>> action,
        CancellationToken cancellationToken
    )
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
