namespace Application.Services.Wishlist.DTOs.Request;

public record DeleteWishlistItemRequest
{
    public required Guid WishlistItemId { get; init; }
    public required Guid UserId { get; init; }
}
