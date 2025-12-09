using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.PermissionAggregate.ValueObjects;

namespace Overclocked.Domain.PermissionAggregate;

public class Permission : AggregateRoot<PermissionId>
{
    public string Name { get; private set; }

    private Permission()
    {
    }
    private Permission(PermissionId id, string name) : base(id)
    {
        Name = name;
    }

    public static Permission Create(PermissionId id, string name)
    {
        return new(id, name);
    }
}
