using Domain.Entities.Common;

namespace Domain.Entities;

public class WishlistItem : Entity
{
    public required Guid WishlistId { get; set; }
    public required Guid ProductId { get; set; }

    // Navigation Properties
    public Wishlist? Wishlist { get; set; }
    public Product? Product { get; set; }
}
