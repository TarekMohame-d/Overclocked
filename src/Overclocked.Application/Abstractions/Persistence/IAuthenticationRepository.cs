using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IAuthenticationRepository : IRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    Task<List<string>> GetPermissionsAsync(Role role, CancellationToken ct = default);

    Task<User?> GetWithRefreshTokensAsync(UserId userId, CancellationToken ct = default);

    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);

    Task<bool> PhoneExistsAsync(string phone, CancellationToken ct = default);

    void Add(User user);
}
