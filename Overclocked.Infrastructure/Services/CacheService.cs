using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Overclocked.Application.Abstraction.Services;
using StackExchange.Redis;

namespace Overclocked.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly IDatabase _redisDb;

    public CacheService(IDistributedCache cache, IConnectionMultiplexer redis)
    {
        _cache = cache;
        _redisDb = redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cachedData = await _cache.GetStringAsync(key, cancellationToken);
        return cachedData is not null ? JsonSerializer.Deserialize<T>(cachedData) : default;
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration = default,
        CancellationToken cancellationToken = default
    )
    {
        var json = JsonSerializer.Serialize(value);
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiration };

        await _cache.SetStringAsync(key, json, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        await _cache.RemoveAsync(key, cancellationToken);

    // Works only with Redis
    public async Task AddToSetAsync(string setKey, string value) => await _redisDb.SetAddAsync(setKey, value);

    public async Task<IEnumerable<string>> GetSetMembersAsync(string setKey)
    {
        RedisValue[] members = await _redisDb.SetMembersAsync(setKey);
        return members.Select(m => m.ToString());
    }

    public async Task RemoveSetAsync(string setKey) => await _redisDb.KeyDeleteAsync(setKey);

    public async Task RemoveKeysInSetAsync(string setKey, CancellationToken cancellationToken = default)
    {
        IEnumerable<string> keys = await GetSetMembersAsync(setKey);

        if(!keys.Any())
        {
            return;
        }

        IEnumerable<Task> removalTasks = keys.Select(x => RemoveAsync(x, cancellationToken));

        await Task.WhenAll(removalTasks);

        await RemoveSetAsync(setKey);
    }
}
