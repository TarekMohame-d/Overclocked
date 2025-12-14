using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context)
    : GenericRepository<User, UserId>(context), IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbSet.AsTracking()
            .Include(u => u.EmailConfirmationCode)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }

    public Task<User?> GetWithRefreshTokensAsync(UserId userId, CancellationToken cancellationToken = default)
    {
        return _dbSet.AsTracking()
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
    }
}
