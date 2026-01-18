using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.ProductAggregate.ValueObjects;

public record ProductImageId(Guid Value) : IEntityKey
{
    public static ProductImageId Create() => new(Guid.CreateVersion7());

    public static ProductImageId Create(Guid value) => new(value);
}
