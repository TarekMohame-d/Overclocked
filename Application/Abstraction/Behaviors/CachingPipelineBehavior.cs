using Application.Abstraction.Messaging;
using Application.Abstraction.Services;
using Application.Common.Results;
using Microsoft.Extensions.Logging;

namespace Application.Abstraction.Behaviors;

public sealed class CachingPipelineBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICachedRequest<TResponse>
    where TResponse : Result
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingPipelineBehavior<TRequest, TResponse>> _logger;

    public CachingPipelineBehavior(ICacheService cacheService, ILogger<CachingPipelineBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request.BypassCache)
        {
            return await next(cancellationToken);
        }

        string requestName = request.GetType().Name;

        string cacheKey = request.CacheKey;

        TResponse? cached = await _cacheService.GetAsync<TResponse>(cacheKey, cancellationToken);

        if (cached is not null)
        {
            _logger.LogInformation("Cache hit for {RequestName}", requestName);
            return cached;
        }

        _logger.LogInformation("Cache miss for {RequestName}", requestName);

        TResponse response = await next(cancellationToken);

        if (response.IsSuccess)
        {
            await _cacheService.SetAsync(cacheKey, response, request.SlidingExpiration, cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.CacheSetKey))
            {
                await _cacheService.AddToSetAsync(request.CacheSetKey, cacheKey);
            }
        }

        return response;
    }
}
