namespace Application.Services.Cart.DTOs.Request;

public record UpdateCartItemRequest
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
}
