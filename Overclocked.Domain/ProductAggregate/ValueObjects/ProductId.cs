using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.ProductAggregate.ValueObjects;

public record ProductId(Guid Value) : IEntityKey
{
    public static ProductId Create() => new(Guid.CreateVersion7());
    public static ProductId Create(Guid value) => new(value);
    public static implicit operator Guid(ProductId id) => id.Value;
}
