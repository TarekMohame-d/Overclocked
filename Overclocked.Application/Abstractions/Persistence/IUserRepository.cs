using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IUserRepository : IGenericRepository<User, UserId>
{
    Task<List<string>> GetPermissionsByRoleAsync(Role role, CancellationToken cancellationToken);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetWithRefreshTokensAsync(UserId userId, CancellationToken cancellationToken = default);
}
