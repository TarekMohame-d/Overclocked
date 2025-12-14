using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IUserRepository : IGenericRepository<User, UserId>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetWithRefreshTokensAsync(UserId userId, CancellationToken cancellationToken = default);
}
