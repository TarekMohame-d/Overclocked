using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.TagAggregate.ValueObjects;

public record TagId(Guid Value) : IEntityKey
{
    public static TagId Create() => new(Guid.CreateVersion7());

    public static TagId Create(Guid value) => new(value);

    public static implicit operator Guid(TagId id) => id.Value;
}
