namespace Overclocked.Contracts.Wishlist;

public record AddWishlistItemRequest
{
    public required Guid ProductId { get; init; }
}
