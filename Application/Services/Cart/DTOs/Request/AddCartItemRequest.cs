namespace Application.Services.Cart.DTOs.Request;

public record AddCartItemRequest
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}
