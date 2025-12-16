using Overclocked.Domain.UserAggregate.Enums;

namespace Overclocked.Infrastructure.Persistence.Entities;

public class RolePermissionLookup
{
    public Role RoleId { get; set; }
    public Permission PermissionId { get; set; }
}
