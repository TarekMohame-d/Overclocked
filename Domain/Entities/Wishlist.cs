namespace Domain.Entities;

public class Wishlist
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public required Guid UserId { get; set; }

    // Navigation Properties
    public User? User { get; set; }
    public ICollection<WishlistItem> WishlistItems { get; set; } = [];

    public void AddWishlistItem(Guid productId)
    {
        WishlistItems.Add(
                new WishlistItem
                {
                    WishlistId = Id,
                    ProductId = productId
                });
    }
}
