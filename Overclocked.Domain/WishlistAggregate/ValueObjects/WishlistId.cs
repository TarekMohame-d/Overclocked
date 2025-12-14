using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.WishlistAggregate.ValueObjects;

public record WishlistId(Guid Value) : IEntityKey
{
    public static WishlistId Create() => new(Guid.CreateVersion7());
    public static WishlistId Create(Guid value) => new(value);
    public static implicit operator Guid(WishlistId id) => id.Value;
}
