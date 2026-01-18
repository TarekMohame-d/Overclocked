using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.PaymentAggregate.ValueObjects;

public record PaymentId(Guid Value) : IEntityKey
{
    public static PaymentId Create() => new(Guid.CreateVersion7());

    public static PaymentId Create(Guid value) => new(value);
}
