using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.RoleAggregate.ValueObjects;

namespace Overclocked.Domain.RoleAggregate;

public class Role : AggregateRoot<RoleId>
{
    public string Name { get; private set; }

    // Relationships
    private readonly List<RolePermission> _rolePermissions = [];
    public IReadOnlyCollection<RolePermission> RolePermissions => _rolePermissions.AsReadOnly();

    private Role()
    {
    }
    private Role(RoleId id, string name) : base(id)
    {
        Name = name;
    }

    public static Role Create(RoleId id, string name)
    {
        return new(id, name);
    }
}
