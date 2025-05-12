namespace Domain.Entities
{
    public class WishListProduct
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }

        // Navigations
        public Product Product { get; set; }
        public WishList WishList { get; set; }
    }
}
