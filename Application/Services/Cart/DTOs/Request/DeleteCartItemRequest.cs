namespace Application.Services.Cart.DTOs.Request;

public record DeleteCartItemRequest
{
    public required Guid CartItemId { get; init; }
    public required Guid UserId { get; init; }
}
