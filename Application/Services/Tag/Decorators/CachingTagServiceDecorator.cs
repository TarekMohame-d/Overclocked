using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.DTOs.Response;
using Microsoft.Extensions.Logging;

namespace Application.Services.Tag.Decorators;

public class CachingTagServiceDecorator : ITagService
{
    private readonly ITagService _inner;
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingTagServiceDecorator> _logger;

    public CachingTagServiceDecorator(
        ITagService inner,
        ICacheService cacheService,
        ILogger<CachingTagServiceDecorator> logger)
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

    public Task<Result<PagedResult<TagListResponse>>> GetPagedTagsAsync(
    GetPagedTagsQuery query, CancellationToken cancellationToken)
    => ExecuteWithCacheAsync(query, _inner.GetPagedTagsAsync, cancellationToken);

    public Task<Result<TagResponse>> GetTagByIdAsync(
        GetTagByIdRequest request, CancellationToken cancellationToken)
        => ExecuteWithCacheAsync(request, _inner.GetTagByIdAsync, cancellationToken);

    public async Task<Result> CreateTagAsync(CreateTagRequest request, CancellationToken cancellationToken)
    {
        var result = await _inner.CreateTagAsync(request, cancellationToken);

        if (result.IsSuccess)
            await _cacheService.RemoveKeysInSetAsync(CacheKeys.TagSet, cancellationToken);

        return result;
    }

    public async Task<Result> UpdateTagAsync(UpdateTagRequest request, CancellationToken cancellationToken)
    {
        var result = await _inner.UpdateTagAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            await _cacheService.RemoveAsync(CacheKeys.Tag(request.Id.ToString()), cancellationToken);
            await _cacheService.RemoveKeysInSetAsync(CacheKeys.TagSet, cancellationToken);
        }
        return result;
    }

    public async Task<Result> DeleteTagAsync(DeleteTagRequest request, CancellationToken cancellationToken)
    {
        var result = await _inner.DeleteTagAsync(request, cancellationToken);

        if (result.IsSuccess)
        {
            await _cacheService.RemoveAsync(CacheKeys.Tag(request.Id.ToString()), cancellationToken);
            await _cacheService.RemoveKeysInSetAsync(CacheKeys.TagSet, cancellationToken);
        }

        return result;
    }
}
