using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IUserRepository : IRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken ct);
}
