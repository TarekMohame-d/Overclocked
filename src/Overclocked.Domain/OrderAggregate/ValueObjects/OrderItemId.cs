using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.OrderAggregate.ValueObjects;

public record OrderItemId(Guid Value) : IEntityKey
{
    public static OrderItemId Create() => new(Guid.CreateVersion7());

    public static OrderItemId Create(Guid value) => new(value);

    public static implicit operator Guid(OrderItemId id) => id.Value;
}
