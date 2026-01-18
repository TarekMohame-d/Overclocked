using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Abstractions.Behaviors;

internal static class CachingDecorator
{
    internal sealed class RequestHandler<TRequest, TResponse>(
        IRequestHandler<TRequest, TResponse> innerHandler,
        ICacheService cacheService,
        ILogger<RequestHandler<TRequest, TResponse>> logger
    ) : IRequestHandler<TRequest, TResponse>, IDecorator
        where TRequest : IRequest<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TRequest request, CancellationToken ct)
        {
            // Handle Caching (Read/Write)
            if (request is ICachedRequest cachedRequest)
            {
                logger.LogInformation("Checking cache for {Key}", cachedRequest.CacheKey);

                // Try Get
                TResponse? cachedResult = await cacheService.GetAsync<TResponse>(cachedRequest.CacheKey, ct);

                if (cachedResult is not null)
                {
                    logger.LogInformation("Cache HIT for {Key}", cachedRequest.CacheKey);
                    return Result.Success(cachedResult);
                }

                logger.LogInformation("Cache MISS for {Key}", cachedRequest.CacheKey);
            }

            // Execute Actual Logic (Database call)
            Result<TResponse> result = await innerHandler.Handle(request, ct);

            if (result.IsSuccess && request is ICachedRequest cacheable)
            {
                var cacheKey = cacheable.CacheKey;
                logger.LogInformation("Cache SET for {Key}", cacheKey);
                await cacheService.SetAsync(cacheKey, result.Value, cacheable.Expiration, ct);

                if (!string.IsNullOrWhiteSpace(cacheable.CacheSetKey))
                    await cacheService.AddToSetAsync(cacheable.CacheSetKey, cacheKey);
            }

            if (result.IsSuccess && request is ICacheInvalidatorRequest invalidator)
                await InvalidateCacheAsync(invalidator, cacheService, logger, ct);

            return result;
        }
    }

    internal sealed class RequestHandler<TRequest>(
        IRequestHandler<TRequest> innerHandler,
        ICacheService cacheService,
        ILogger<RequestHandler<TRequest>> logger
    ) : IRequestHandler<TRequest>, IDecorator
        where TRequest : IRequest
    {
        public async Task<Result> Handle(TRequest request, CancellationToken ct)
        {
            Result result = await innerHandler.Handle(request, ct);

            if (result.IsSuccess && request is ICacheInvalidatorRequest invalidator)
                await InvalidateCacheAsync(invalidator, cacheService, logger, ct);

            return result;
        }
    }

    private static async Task InvalidateCacheAsync(
        ICacheInvalidatorRequest invalidator,
        ICacheService cacheService,
        ILogger logger,
        CancellationToken ct
    )
    {
        foreach (var key in invalidator.CacheKeys)
        {
            logger.LogInformation("Invalidating Cache Key: {Key}", key);
            await cacheService.RemoveAsync(key, ct);
        }

        if (string.IsNullOrWhiteSpace(invalidator.CacheSetKey))
            return;

        logger.LogInformation("Invalidating Cache Set: {SetKey}", invalidator.CacheSetKey);
        await cacheService.RemoveKeysInSetAsync(invalidator.CacheSetKey, ct);
    }
}
