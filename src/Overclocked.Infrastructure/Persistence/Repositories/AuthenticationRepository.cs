using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Infrastructure.Persistence.Entities;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class AuthenticationRepository(ApplicationDbContext dbContext) : IAuthenticationRepository
{
    private readonly DbSet<User> _dbSet = dbContext.Users;

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default) =>
        _dbSet
            .AsTracking()
            .AsSplitQuery()
            .Include(u => u.EmailConfirmationCode)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(x => x.Email == email, ct);

    public Task<List<string>> GetPermissionsAsync(Role role, CancellationToken ct = default) =>
        dbContext
            .Set<RolePermissionLookup>()
            .AsNoTracking()
            .Where(rp => rp.RoleId == role)
            .Join(
                inner: dbContext.Set<PermissionLookup>(),
                outerKeySelector: rp => rp.PermissionId,
                innerKeySelector: p => p.Id,
                resultSelector: (rp, p) => p.Name
            )
            .ToListAsync(ct);

    //=======================
    // Another implementation
    //=======================

    // public Task<List<string>> GetPermissionsAsync(Role role, CancellationToken cancellationToken)
    // {
    //     var roleId = (int)role;

    //     IRequestable<string> request = from rp in context.Set<RolePermissionLookup>()
    //                                join p in context.Set<PermissionLookup>() on rp.PermissionId equals p.Id
    //                                where rp.RoleId == roleId
    //                                select p.Name;

    //     return request.AsNoTracking().ToListAsync(cancellationToken);
    // }

    public Task<User?> GetWithRefreshTokensAsync(UserId userId, CancellationToken ct = default) =>
        _dbSet.AsTracking().Include(u => u.RefreshTokens).FirstOrDefaultAsync(x => x.Id == userId, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct = default) =>
        _dbSet.AnyAsync(x => x.Email == email, ct);

    public Task<bool> PhoneExistsAsync(string phone, CancellationToken ct = default) =>
        _dbSet.AnyAsync(x => x.Phone == phone, ct);

    public void Add(User user) => _dbSet.Add(user);
}
