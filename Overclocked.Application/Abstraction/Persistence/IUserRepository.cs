using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Application.Abstraction.Persistence;

public interface IUserRepository : IGenericRepository<User, UserId>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
