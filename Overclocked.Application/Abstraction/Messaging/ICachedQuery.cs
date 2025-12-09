namespace Overclocked.Application.Abstraction.Messaging;

public interface ICachedQuery
{
    string CacheKey { get; }
    string? CacheSetKey { get; }
    TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}
