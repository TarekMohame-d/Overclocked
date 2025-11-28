namespace Application.Services.Wishlist.DTOs.Request;

public record AddWishlistItemRequestBody
{
    public required Guid ProductId { get; init; }
}

public record AddWishlistItemRequest : AddWishlistItemRequestBody
{
    public required Guid UserId { get; init; }
}
