namespace Domain.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        // Navigations
        public Customer Customer { get; set; }
        public ICollection<CartProduct> CartProducts { get; set; }
    }
}
