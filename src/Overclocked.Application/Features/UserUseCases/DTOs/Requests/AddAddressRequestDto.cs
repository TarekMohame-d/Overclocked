namespace Overclocked.Application.Features.UserUseCases.DTOs.Requests;

public record AddAddressRequestDto(
    int Apartment,
    string Building,
    string Street,
    string City,
    string PostalCode,
    string Description
);
