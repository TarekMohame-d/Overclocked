using Overclocked.Domain.PermissionAggregate.ValueObjects;
using Overclocked.Domain.RoleAggregate.ValueObjects;

namespace Overclocked.Application.Abstraction.Persistence;

public interface IPermissionRepository : IGenericRepository<Domain.PermissionAggregate.Permission, PermissionId>
{
    Task<List<string>> GetPermissionsByRoleIdAsync(RoleId roleId, CancellationToken cancellationToken);
}
