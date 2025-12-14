namespace Overclocked.Contracts.Wishlist;

public record WishlistResponse
{
    public required IEnumerable<WishlistItemResponse> WishlistItems { get; init; }

    public record WishlistItemResponse
    {
        public required Guid ProductId { get; init; }
        public required string ProductName { get; init; }
        public required string ProductDescription { get; init; }
        public required string ProductThumbnail { get; init; }
    }
}
