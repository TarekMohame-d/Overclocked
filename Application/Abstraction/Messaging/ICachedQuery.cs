namespace Application.Abstraction.Messaging;

public interface ICachedQuery<TResponse> : IQuery<TResponse>
{
    string CacheKey { get; }
    bool BypassCache { get; }
    TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}

