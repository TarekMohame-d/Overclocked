namespace Overclocked.Application.Features.OrderUseCases.DTOs.Requests;

public record ShippingAddressRequestDto(
    int Apartment,
    string Building,
    string Street,
    string City,
    string PostalCode,
    string Description
);
