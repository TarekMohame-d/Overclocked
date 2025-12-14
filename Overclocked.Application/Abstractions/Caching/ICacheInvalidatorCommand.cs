namespace Overclocked.Application.Abstractions.Caching;

public interface ICacheInvalidatorCommand
{
    string[] CacheKeys { get; }
    string? CacheSetKey { get; }
}
