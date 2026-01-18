namespace Overclocked.Application.Abstractions.Caching;

public interface ICachedRequest
{
    string CacheKey { get; }
    string? CacheSetKey { get; }
    TimeSpan Expiration { get; }
}
