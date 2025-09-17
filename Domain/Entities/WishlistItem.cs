using Domain.Entities.Common;

namespace Domain.Entities;

public class WishlistItem : BaseEntity
{
    public Guid WishlistId { get; set; }
    public Guid ProductId { get; set; }

    // Navigation Properties
    public Wishlist? Wishlist { get; set; }
    public Product? Product { get; set; }
}
