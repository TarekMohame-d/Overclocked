using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.ProductsAggregate.ValueObjects;

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
}
