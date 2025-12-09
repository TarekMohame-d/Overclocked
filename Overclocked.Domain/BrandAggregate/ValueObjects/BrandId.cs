using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.BrandAggregate.ValueObjects;

public record BrandId(Guid Value) : IEntityKey
{
    public static BrandId Create() => new(Guid.CreateVersion7());
    public static BrandId Create(Guid value) => new(value);
    public static implicit operator Guid(BrandId id) => id.Value;
}
