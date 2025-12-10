using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.PermissionAggregate.ValueObjects;

public record PermissionId(int Value) : IEntityKey
{
    public static PermissionId Create(int value) => new(value);
    public static implicit operator int(PermissionId id) => id.Value;
}
