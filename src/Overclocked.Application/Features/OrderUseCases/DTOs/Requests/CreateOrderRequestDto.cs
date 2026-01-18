namespace Overclocked.Application.Features.OrderUseCases.DTOs.Requests;

public record CreateOrderRequestDto(ShippingAddressRequestDto ShippingAddress, string PaymentProvider, string PaymentMethod);
