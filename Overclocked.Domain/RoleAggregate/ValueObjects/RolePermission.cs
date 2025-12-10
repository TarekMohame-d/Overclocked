using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.PermissionAggregate.ValueObjects;

namespace Overclocked.Domain.RoleAggregate.ValueObjects;

public record RolePermission : IValueObject
{
    public PermissionId PermissionId { get; private set; }

    private RolePermission()
    {
    }

    private RolePermission(PermissionId permissionId)
    {
        PermissionId = permissionId;
    }

    public static RolePermission Create(PermissionId permissionId) => new(permissionId);
}
