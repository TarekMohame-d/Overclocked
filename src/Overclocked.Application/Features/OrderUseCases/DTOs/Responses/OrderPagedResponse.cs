namespace Overclocked.Application.Features.OrderUseCases.DTOs.Responses;

public record OrderPagedResponse
{
    public required Guid OrderId { get; init; }
    public required string OrderNumber { get; init; }
    public required DateTimeOffset OrderDate { get; init; }
    public required bool CanBeCancelled { get; init; }
    public required string OrderStatus { get; init; }
    public required IReadOnlyList<OrderItemResponse> OrderItems { get; init; }
    public required decimal Total { get; init; }
}
