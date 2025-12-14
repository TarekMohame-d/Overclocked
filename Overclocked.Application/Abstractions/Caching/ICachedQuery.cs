namespace Overclocked.Application.Abstractions.Caching;

public interface ICachedQuery
{
    string CacheKey { get; }
    string? CacheSetKey { get; }
    TimeSpan Expiration { get; }
}
