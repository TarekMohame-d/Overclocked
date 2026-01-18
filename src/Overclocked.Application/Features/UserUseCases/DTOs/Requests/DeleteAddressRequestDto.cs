namespace Overclocked.Application.Features.UserUseCases.DTOs.Requests;

public record DeleteAddressRequestDto(
    int Apartment,
    string Building,
    string Street,
    string City,
    string PostalCode,
    string Description
);
