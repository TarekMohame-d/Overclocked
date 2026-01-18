namespace Overclocked.Application.Features.OrderUseCases.DTOs.Responses;

public record OrderItemResponse
{
    public required Guid OrderItemId { get; init; }
    public required Guid ProductId { get; init; }
    public required string ProductName { get; init; }
    public required string ProductThumbnail { get; init; }
    public required int Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
    public decimal LineTotal => Math.Round(UnitPrice * Quantity, 2, MidpointRounding.ToEven);
}
