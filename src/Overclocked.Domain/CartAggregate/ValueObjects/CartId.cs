using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.CartAggregate.ValueObjects;

public record CartId(Guid Value) : IEntityKey
{
    public static CartId Create() => new(Guid.CreateVersion7());

    public static CartId Create(Guid value) => new(value);
}
