using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.Common.Shared.ValueObjects.Money;

public record DiscountRate : IValueObject
{
    public decimal Value { get; private init; }

    private DiscountRate() { }

    public static Result<DiscountRate> Create(decimal value)
    {
        if (value is < 0 or > 0.99m)
            return Result.Failure<DiscountRate>(Error.Validation("DiscountRate.Value", "Value must be between 0 and 0.99."));

        return Result.Success(new DiscountRate { Value = Math.Round(value, 2, MidpointRounding.ToEven) });
    }

    // For EF
    internal static DiscountRate Load(decimal value) => new() { Value = Math.Round(value, 2, MidpointRounding.ToEven) };

    public static DiscountRate Zero => new() { Value = 0.00m };
}
