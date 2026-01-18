using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext dbContext) : IUserRepository
{
    private readonly DbSet<User> _dbSet = dbContext.Users;

    public Task<User?> GetByIdAsync(UserId id, CancellationToken ct) => _dbSet.FindAsync([id], ct).AsTask();
}
