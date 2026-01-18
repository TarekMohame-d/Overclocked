namespace Overclocked.Application.Abstractions.Services;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cs = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cs = default);
    Task RemoveAsync(string key, CancellationToken cs = default);

    // sets used to store sets os same values
    // Works only with Redis
    Task AddToSetAsync(string setKey, string value);
    Task<IEnumerable<string>> GetSetMembersAsync(string setKey);
    Task RemoveSetAsync(string setKey);
    Task RemoveKeysInSetAsync(string setKey, CancellationToken cs = default);
}
