using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.ReviewAggregate.ValueObjects;

public record ReviewId(Guid Value) : IEntityKey
{
    public static ReviewId Create() => new(Guid.CreateVersion7());

    public static ReviewId Create(Guid value) => new(value);
}
