using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context)
    : GenericRepository<User, UserId>(context), IUserRepository
{
    private readonly ApplicationDbContext _context = context;
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return _context.Users.AsTracking().AsSplitQuery()
            .SingleOrDefaultAsync(x => x.Email == email, cancellationToken);
    }
}
