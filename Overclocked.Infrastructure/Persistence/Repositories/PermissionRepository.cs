using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Domain.PermissionAggregate;
using Overclocked.Domain.PermissionAggregate.ValueObjects;
using Overclocked.Domain.RoleAggregate;
using Overclocked.Domain.RoleAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class PermissionRepository(ApplicationDbContext context)
    : GenericRepository<Permission, PermissionId>(context), IPermissionRepository
{
    private readonly ApplicationDbContext _dbContext = context;
    public Task<List<string>> GetPermissionsByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken)
    {
        Task<List<string>> permissionNames = _dbContext.Set<Permission>()
            .AsNoTracking()
            .AsSplitQuery()
            .Where(p => _dbContext.Set<Role>()
                .Where(r => r.Id == roleId)
                .SelectMany(r => r.RolePermissions)
                .Any(rp => rp.PermissionId == p.Id))
            .Select(p => p.Name)
            .ToListAsync(cancellationToken);

        return permissionNames;
    }
}
