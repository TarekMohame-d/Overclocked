using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Overclocked.Application.Abstractions.Services;
using StackExchange.Redis;

namespace Overclocked.Infrastructure.Services;

public class CacheService(IDistributedCache cache, IConnectionMultiplexer redis) : ICacheService
{
    private readonly IDistributedCache _cache = cache;
    private readonly IDatabase _redisDb = redis.GetDatabase();

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        WriteIndented = false,
    };

    public async Task<T?> GetAsync<T>(string key, CancellationToken cs = default)
    {
        var cachedData = await _cache.GetStringAsync(key, cs);
        return cachedData is not null ? JsonSerializer.Deserialize<T>(cachedData, _jsonOptions) : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cs = default)
    {
        var json = JsonSerializer.Serialize(value, _jsonOptions);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5),
        };

        await _cache.SetStringAsync(key, json, options, cs);
    }

    public async Task RemoveAsync(string key, CancellationToken cs = default) => await _cache.RemoveAsync(key, cs);

    // Works only with Redis
    public async Task AddToSetAsync(string setKey, string value) => await _redisDb.SetAddAsync(setKey, value);

    public async Task<IEnumerable<string>> GetSetMembersAsync(string setKey)
    {
        RedisValue[] members = await _redisDb.SetMembersAsync(setKey);
        return members.Select(m => m.ToString());
    }

    public async Task RemoveSetAsync(string setKey) => await _redisDb.KeyDeleteAsync(setKey);

    public async Task RemoveKeysInSetAsync(string setKey, CancellationToken cs = default)
    {
        RedisValue[] members = await _redisDb.SetMembersAsync(setKey);

        if (members.Length == 0)
            return;

        RedisKey[] keysToDelete = members.Select(m => (RedisKey)m.ToString()).Concat([setKey]).ToArray();

        await _redisDb.KeyDeleteAsync(keysToDelete);
    }
}
