using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.Common.Shared.ValueObjects.Address;

public record Address : IValueObject
{
    private Address() { }

    public int Apartment { get; private init; }
    public string Building { get; private init; } = null!;
    public string Street { get; private init; } = null!;
    public string City { get; private init; } = null!;
    public string PostalCode { get; private init; } = null!;
    public string Description { get; private init; } = null!;

    public static Result<Address> Create(
        int apartment,
        string building,
        string street,
        string city,
        string postalCode,
        string description
    )
    {
        Result validationResult = ValidateState(apartment, building, street, city, postalCode, description);

        if (validationResult.IsFailure)
            return Result.Failure<Address>(validationResult.Error);

        var address = new Address
        {
            Apartment = apartment,
            Building = building,
            Street = street,
            City = city,
            PostalCode = postalCode,
            Description = description,
        };

        return Result.Success(address);
    }

    // For EF
    internal static Address Load(string street, string city, string postalCode, string description) =>
        new()
        {
            Street = street,
            City = city,
            PostalCode = postalCode,
            Description = description,
        };

    private static Result ValidateState(
        int apartment,
        string building,
        string street,
        string city,
        string postalCode,
        string description
    )
    {
        if (apartment <= 0)
            return Result.Failure(Error.Validation("Address.Apartment", "Apartment must be greater than 0."));

        if (string.IsNullOrWhiteSpace(building) || building.Length > 30)
            return Result.Failure(
                Error.Validation("Address.Building", "Building is required and must be less than 30 characters.")
            );

        if (string.IsNullOrWhiteSpace(street) || street.Length > 100)
            return Result.Failure(Error.Validation("Address.Street", "Street is required and must be less than 100 characters."));

        if (string.IsNullOrWhiteSpace(city) || city.Length > 50)
            return Result.Failure(Error.Validation("Address.City", "City is required and must be less than 50 characters."));

        if (string.IsNullOrWhiteSpace(postalCode) || postalCode.Length > 10)
            return Result.Failure(
                Error.Validation("Address.PostalCode", "PostalCode is required and must be less than 10 characters.")
            );

        if (string.IsNullOrWhiteSpace(description) || description.Length > 300)
            return Result.Failure(
                Error.Validation("Address.Description", "Description is required and must be less than 300 characters.")
            );

        return Result.Success();
    }
}
