using Domain.Entities.Common;

namespace Domain.Entities;

public class Product : BaseEntity
{
    public Guid CategoryId { get; set; }
    public Guid BrandId { get; set; }
    public required string Name { get; set; }
    public string NormalizedName { get; set; } = default!;
    public required string Thumbnail { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public decimal Rating { get; set; }
    public int StockQuantity { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation Properties
    public Category? Category { get; set; }
    public Brand? Brand { get; set; }
    public ICollection<CartItem>? CartItems { get; set; }
    public ICollection<WishlistItem>? WishlistItems { get; set; }
    public ICollection<Review>? Reviews { get; set; }
    public ICollection<OrderItem>? OrderItems { get; set; }
    public ICollection<InvoiceItem>? InvoiceItems { get; set; }
    public ICollection<TagProduct>? TagProducts { get; set; }
    public ICollection<ProductImage>? ProductImages { get; set; }
    public ICollection<Specification>? Specifications { get; set; }
}
