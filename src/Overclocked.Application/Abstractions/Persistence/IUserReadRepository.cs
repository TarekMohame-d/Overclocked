using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface IUserReadRepository : IRepository
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken);
}
