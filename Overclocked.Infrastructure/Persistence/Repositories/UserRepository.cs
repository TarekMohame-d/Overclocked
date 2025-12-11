using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context)
    : GenericRepository<User, UserId>(context), IUserRepository
{
    private readonly ApplicationDbContext _dbContext = context;
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Users.AsTracking()
            .Include(u => u.EmailConfirmationCode)
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
    }
}
