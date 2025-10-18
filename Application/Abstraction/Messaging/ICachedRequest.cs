namespace Application.Abstraction.Messaging;

public interface ICachedRequest<TResponse> : IQuery<TResponse>
{
    string CacheKey { get; }
    string? CacheSetKey { get; }
    bool BypassCache { get; }
    TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}
