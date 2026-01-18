using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.UserAggregate.ValueObjects;

public record RefreshTokenId(Guid Value) : IEntityKey
{
    public static RefreshTokenId Create() => new(Guid.CreateVersion7());

    public static RefreshTokenId Create(Guid value) => new(value);
}
