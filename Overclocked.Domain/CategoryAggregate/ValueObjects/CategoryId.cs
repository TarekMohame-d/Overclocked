using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.CategoryAggregate.ValueObjects;

public record CategoryId(Guid Value) : IEntityKey
{
    public static CategoryId Create() => new(Guid.CreateVersion7());
    public static CategoryId Create(Guid value) => new(value);
    public static implicit operator Guid(CategoryId id) => id.Value;
}
