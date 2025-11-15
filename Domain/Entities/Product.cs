using Domain.Entities.Common;

namespace Domain.Entities;

public class Product : Entity
{
    public required Guid CategoryId { get; set; }
    public required Guid BrandId { get; set; }
    public required string Name { get; set; }
    public string NormalizedName { get; init; } = string.Empty;
    public required string Thumbnail { get; set; }
    public required string Description { get; set; }
    public required decimal Price { get; set; }
    public required decimal Discount { get; set; }
    public required double Rating { get; set; }
    public required int StockQuantity { get; set; }
    public required bool IsDeleted { get; set; }

    // Navigation Properties
    public Brand? Brand { get; set; }
    public Category? Category { get; set; }
    public ICollection<CartItem> CartItems { get; set; } = [];
    public ICollection<WishlistItem> WishlistItems { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public ICollection<TagProduct> TagProducts { get; set; } = [];
    public ICollection<ProductImage> ProductImages { get; set; } = [];
    public ICollection<Specification> Specifications { get; set; } = [];
}
