using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.CartAggregate.ValueObjects;

public record CartItemId(Guid Value) : IEntityKey
{
    public static CartItemId Create() => new(Guid.CreateVersion7());
    public static CartItemId Create(Guid value) => new(value);
    public static implicit operator Guid(CartItemId id) => id.Value;
}
