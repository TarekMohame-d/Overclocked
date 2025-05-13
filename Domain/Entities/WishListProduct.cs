namespace Domain.Entities
{
    public class WishlistProduct
    {
        public Guid WishlistId { get; set; }
        public Guid ProductId { get; set; }

        // Navigations
        public Product Product { get; set; }
        public Wishlist Wishlist { get; set; }
    }
}
