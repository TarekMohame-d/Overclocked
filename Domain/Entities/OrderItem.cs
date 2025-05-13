using Domain.Entities.Common;

namespace Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Navigations
        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
