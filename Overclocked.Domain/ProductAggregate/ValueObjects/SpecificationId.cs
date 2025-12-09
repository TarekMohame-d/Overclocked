using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.ProductAggregate.ValueObjects;

public record SpecificationId(Guid Value) : IEntityKey
{
    public static SpecificationId Create() => new(Guid.CreateVersion7());
    public static SpecificationId Create(Guid value) => new(value);
    public static implicit operator Guid(SpecificationId id) => id.Value;
}
