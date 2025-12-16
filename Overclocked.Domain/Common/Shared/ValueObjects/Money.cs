using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.Common.Shared.ValueObjects;

public record Money : IValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private Money()
    {
    }

    private Money(decimal amount, string currency = "USD")
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money Create(decimal amount, string currency = "USD")
    {
        return new(amount, currency);
    }

    public static Money Zero => new(0);

    public static Money operator +(Money a, Money b)
    {
        if(a.Currency != b.Currency)
            throw new Exception("Currency mismatch");
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator *(Money a, int multiplier)
        => new(a.Amount * multiplier, a.Currency);
}
