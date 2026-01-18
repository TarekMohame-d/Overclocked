using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.ProductAggregate.Entities;

public sealed class Specification : Entity<SpecificationId>
{
    private Specification() { }

    private Specification(SpecificationId id, string name, string value)
        : base(id)
    {
        Name = name;
        NormalizedName = name.ToUpperInvariant();
        Value = value;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public string Name { get; private set; } = null!;
    public string NormalizedName { get; private set; } = null!;
    public string Value { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Result<Specification> Create(string name, string value)
    {
        name = name.Trim();
        value = value.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure<Specification>(ProductErrors.SpecificationNameIsRequired);

        if (name.Length > 50)
            return Result.Failure<Specification>(ProductErrors.SpecificationNameIsTooLong);

        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure<Specification>(ProductErrors.SpecificationValueIsRequired);

        if (value.Length > 300)
            return Result.Failure<Specification>(ProductErrors.SpecificationValueIsTooLong);

        return Result.Success(new Specification(SpecificationId.Create(), name, value));
    }

    public Result UpdateValue(string value)
    {
        value = value.Trim();

        if (string.IsNullOrWhiteSpace(value))
            return Result.Failure(ProductErrors.SpecificationValueIsRequired);

        if (value.Length > 300)
            return Result.Failure(ProductErrors.SpecificationValueIsTooLong);

        Value = value;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }
}
