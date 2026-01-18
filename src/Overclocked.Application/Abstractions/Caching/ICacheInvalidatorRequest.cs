namespace Overclocked.Application.Abstractions.Caching;

public interface ICacheInvalidatorRequest
{
    string[] CacheKeys { get; }
    string? CacheSetKey { get; }
}
