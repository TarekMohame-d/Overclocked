using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class UserReadRepository(ApplicationDbContext dbContext) : IUserReadRepository
{
    private readonly IQueryable<User> _queryable = dbContext.Users.AsNoTracking();

    public Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken) =>
        _queryable.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}
