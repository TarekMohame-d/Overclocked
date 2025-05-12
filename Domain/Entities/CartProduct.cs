namespace Domain.Entities
{
    public class CartProduct
    {
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }

        // Navigations
        public Product Product { get; set; }
        public Cart Cart { get; set; }
    }
}
