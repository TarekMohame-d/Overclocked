using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Tag.Queries.GetTags;
using Overclocked.Contracts.Tag;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Tag.Queries.Decorators;

public class CachingTagQueriesDecorator(
    ITagQueries inner,
    ICacheService cacheService,
    ILogger<CachingTagQueriesDecorator> logger) : ITagQueries
{
    public async Task<Result<PagedResult<TagListResponse>>> GetPagedTagsQueryHandler(GetPagedTagsQuery query, CancellationToken cancellationToken)
    {
        var cacheKey = query.CacheKey;

        // Try get from cache
        PagedResult<TagListResponse>? cachedValue = await cacheService
            .GetAsync<PagedResult<TagListResponse>>(cacheKey, cancellationToken);

        if(cachedValue is not null)
        {
            logger.LogInformation("Cache hit for {QueryName}", nameof(GetPagedTagsQuery));
            return Result<PagedResult<TagListResponse>>.Success(cachedValue);
        }

        logger.LogInformation("Cache miss for {QueryName}", nameof(GetPagedTagsQuery));
        Result<PagedResult<TagListResponse>> result = await inner.GetPagedTagsQueryHandler(query, cancellationToken);

        if(result.IsSuccess)
        {
            logger.LogInformation("Adding cache for Tags Paged: {PagedKey}", cacheKey);
            await cacheService.SetAsync(cacheKey, result.Value, query.SlidingExpiration, cancellationToken);
            logger.LogInformation("Added cache for Tags Paged: {PagedKey}", cacheKey);

            if(!string.IsNullOrWhiteSpace(query.CacheSetKey))
            {
                logger.LogInformation("Adding cache for Tags Set: {TagSet}", CacheKeys.TagSet);
                await cacheService.AddToSetAsync(query.CacheSetKey, cacheKey);
                logger.LogInformation("Added cache for Tags Set: {TagSet}", CacheKeys.TagSet);
            }
        }

        return result;
    }
}
