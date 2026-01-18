using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.Common.Shared.ValueObjects.Money;

public record Money : IValueObject
{
    public decimal Value { get; private init; } = default!;
    public string Currency { get; private init; } = default!;

    private Money() { }

    public static Result<Money> Create(decimal value, string currency = "EGP")
    {
        if (value <= 0.0m)
            return Result.Failure<Money>(Error.Validation("Money.Amount", "Amount must be greater than 0."));

        if (string.IsNullOrWhiteSpace(currency) || currency.Length > 3)
            return Result.Failure<Money>(
                Error.Validation("Money.Currency", "Currency is required and must be less than 3 characters.")
            );

        var roundedValue = Math.Round(value, 2, MidpointRounding.ToEven);

        var money = new Money { Value = roundedValue, Currency = currency };

        return Result.Success(money);
    }

    // For EF
    internal static Money Load(decimal value, string currency) =>
        new() { Value = Math.Round(value, 2, MidpointRounding.ToEven), Currency = currency };

    public static Money Zero => new() { Value = 0.00m, Currency = "EGP" };

    public static Money operator +(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Currency mismatch");

        return new Money { Value = Math.Round(a.Value + b.Value, 2, MidpointRounding.ToEven), Currency = a.Currency };
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        var newAmount = Math.Round(money.Value * multiplier, 2, MidpointRounding.ToEven);

        return new Money { Value = newAmount, Currency = money.Currency };
    }

    public static Money operator -(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new InvalidOperationException("Currency mismatch");

        return new Money { Value = Math.Round(a.Value - b.Value, 2, MidpointRounding.ToEven), Currency = a.Currency };
    }

    public static bool operator >(Money a, Money b) => a.Value > b.Value;

    public static bool operator <(Money a, Money b) => a.Value < b.Value;

    public static bool operator >=(Money a, Money b) => a.Value >= b.Value;

    public static bool operator <=(Money a, Money b) => a.Value <= b.Value;
}
