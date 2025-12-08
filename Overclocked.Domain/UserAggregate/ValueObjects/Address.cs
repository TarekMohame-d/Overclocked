using Overclocked.Domain.Common.Primitives;

namespace Overclocked.Domain.UserAggregate.ValueObjects;

public record Address : IValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string PostalCode { get; private set; }
    public string Description { get; private set; }

    private Address()
    {
    }
    private Address(string street, string city, string postalCode, string description)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        Description = description;
    }

    public static Address Create(string street, string city, string postalCode, string description)
    {
        return new(street, city, postalCode, description);
    }
}
