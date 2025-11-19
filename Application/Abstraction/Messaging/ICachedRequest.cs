namespace Application.Abstraction.Messaging;

public interface ICachedRequest
{
    string CacheKey { get; }
    string? CacheSetKey { get; }
    TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}
