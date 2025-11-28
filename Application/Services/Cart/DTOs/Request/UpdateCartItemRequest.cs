namespace Application.Services.Cart.DTOs.Request;

public record UpdateCartItemRequestBody
{
    public required int Quantity { get; init; }
}

public record UpdateCartItemRequest : UpdateCartItemRequestBody
{
    public required Guid UserId { get; init; }
    public required Guid CartItemId { get; init; }
}
