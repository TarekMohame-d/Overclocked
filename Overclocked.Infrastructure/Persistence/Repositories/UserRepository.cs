using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Infrastructure.Persistence.Entities;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext context)
    : GenericRepository<User, UserId>(context), IUserRepository
{
    public Task<List<string>> GetPermissionsByRoleAsync(Role role, CancellationToken cancellationToken)
    {
        Task<List<string>> permissions = _dbContext.Set<RolePermissionLookup>()
            .AsNoTracking()
            .Where(rp => rp.RoleId == role)
            .Join(
                inner: _dbContext.Set<PermissionLookup>(),
                outerKeySelector: rp => rp.PermissionId,
                innerKeySelector: p => p.Id,
                resultSelector: (rp, p) => p.Name
            )
            .ToListAsync(cancellationToken);

        return permissions;
    }

    //=======================
    // Another implementation
    //=======================

    // public Task<List<string>> GetPermissionsByRoleIdAsync(Role role, CancellationToken cancellationToken)
    // {
    //     var roleId = (int)role;

    //     IQueryable<string> query = from rp in context.Set<RolePermissionLookup>()
    //                                join p in context.Set<PermissionLookup>() on rp.PermissionId equals p.Id
    //                                where rp.RoleId == roleId
    //                                select p.Name;

    //     return query.AsNoTracking().ToListAsync(cancellationToken);
    // }

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
