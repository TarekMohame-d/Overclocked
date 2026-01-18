namespace Overclocked.Application.Features.OrderUseCases.DTOs.Responses;

public record CreateOrderResponse
{
    public required Guid OrderId { get; init; }
    public string? RedirectUrl { get; init; }
    public required bool PaymentPending { get; init; }
}
