namespace Domain.Entities
{
    public class Wishlist
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        // Navigations
        public Customer Customer { get; set; }
        public ICollection<WishlistProduct> WishlistProducts { get; set; }
    }
}
