namespace Domain.Entities
{
    public class WishList
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        // Navigations
        public Customer Customer { get; set; }
        public ICollection<WishListProduct> WishListProducts { get; set; }
    }
}
