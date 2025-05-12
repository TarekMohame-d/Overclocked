using Domain.Entities.Common;

namespace Domain.Entities
{
    public class Product : BaseEntity
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        public Guid BrandId { get; set; }

        // Navigations
        public Category Category { get; set; }
        public Brand Brand { get; set; }
        public ICollection<CartProduct> CartProducts { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<TagProduct> TagProducts { get; set; }
        public ICollection<ProductImage> ProductImages { get; set; }
        public ICollection<Specification> Specifications { get; set; }
        public ICollection<WishListProduct> WishListProducts { get; set; }
    }
}
