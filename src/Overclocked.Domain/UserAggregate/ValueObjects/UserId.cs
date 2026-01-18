using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.UserAggregate.ValueObjects;

public record UserId(Guid Value) : IEntityKey
{
    public static UserId Create() => new(Guid.CreateVersion7());

    public static UserId Create(Guid value) => new(value);
}
