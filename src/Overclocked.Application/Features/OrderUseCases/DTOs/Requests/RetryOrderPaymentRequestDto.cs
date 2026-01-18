namespace Overclocked.Application.Features.OrderUseCases.DTOs.Requests;

public record RetryOrderPaymentRequestDto(
    ShippingAddressRequestDto ShippingAddress,
    string PaymentProvider,
    string PaymentMethod
);
