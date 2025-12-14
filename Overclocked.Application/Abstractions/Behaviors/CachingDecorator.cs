using Microsoft.Extensions.Logging;
using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Abstractions.Behaviors;

internal static class CachingDecorator
{
    internal sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> innerHandler,
        ICacheService cacheService,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            Result<TResponse> result = await innerHandler.Handle(command, cancellationToken);

            if(result.IsSuccess && command is ICacheInvalidatorCommand invalidator)
            {
                await InvalidateCacheAsync(invalidator, cacheService, logger, cancellationToken);
            }

            return result;
        }
    }

    internal sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> innerHandler,
        ICacheService cacheService,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            Result result = await innerHandler.Handle(command, cancellationToken);

            if(result.IsSuccess && command is ICacheInvalidatorCommand invalidator)
            {
                await InvalidateCacheAsync(invalidator, cacheService, logger, cancellationToken);
            }

            return result;
        }
    }

    internal sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> innerHandler,
        ICacheService cacheService,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            if(query is not ICachedQuery cachedQuery)
            {
                return await innerHandler.Handle(query, cancellationToken);
            }

            var cacheKey = cachedQuery.CacheKey;

            TResponse? cachedResult = await cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);
            if(cachedResult is not null)
            {
                logger.LogInformation("Cache HIT for {Key}", cacheKey);
                return Result.Success(cachedResult);
            }

            logger.LogInformation("Cache MISS for {Key}", cacheKey);
            Result<TResponse> result = await innerHandler.Handle(query, cancellationToken);

            if(result.IsSuccess)
            {
                logger.LogInformation("Cache SET for {Key}", cacheKey);
                await cacheService.SetAsync(cacheKey, result.Value, cachedQuery.Expiration, cancellationToken);

                if(!string.IsNullOrWhiteSpace(cachedQuery.CacheSetKey))
                {
                    await cacheService.AddToSetAsync(cachedQuery.CacheSetKey, cacheKey);
                }
            }

            return result;
        }
    }

    private static async Task InvalidateCacheAsync(
        ICacheInvalidatorCommand invalidator,
        ICacheService cacheService,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        foreach(var key in invalidator.CacheKeys)
        {
            logger.LogInformation("Invalidating Cache Key: {Key}", key);
            await cacheService.RemoveAsync(key, cancellationToken);
        }

        if(!string.IsNullOrWhiteSpace(invalidator.CacheSetKey))
        {
            logger.LogInformation("Invalidating Cache Set: {SetKey}", invalidator.CacheSetKey);
            await cacheService.RemoveKeysInSetAsync(invalidator.CacheSetKey, cancellationToken);
        }
    }
}
