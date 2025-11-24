namespace Application.Services.Wishlist.DTOs.Request;

public record AddWishlistItemRequest
{
    public required Guid ProductId { get; init; }
}
