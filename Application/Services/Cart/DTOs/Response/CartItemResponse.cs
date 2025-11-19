namespace Application.Services.Cart.DTOs.Response;

public record CartItemResponse
{
    public required Guid ProductId { get; init; }
    public required string ProductName { get; init; }
    public required int Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
    public required decimal Discount { get; init; }
    public required decimal LineTotal { get; init; }
}
