using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.OrderAggregate.ValueObjects;

public record OrderId(Guid Value) : IEntityKey
{
    public static OrderId Create() => new(Guid.CreateVersion7());
    public static OrderId Create(Guid value) => new(value);
    public static implicit operator Guid(OrderId id) => id.Value;
}
