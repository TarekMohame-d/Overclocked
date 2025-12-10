using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.RoleAggregate.ValueObjects;

public record RoleId(int Value) : IEntityKey
{
    public static RoleId Create(int value) => new(value);
    public static implicit operator int(RoleId id) => id.Value;
}
