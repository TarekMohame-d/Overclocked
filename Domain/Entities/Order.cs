using Domain.Entities.Common;

namespace Domain.Entities
{
    public class Order : BaseEntity
    {
        public decimal TotalPrice { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ShipmentId { get; set; }
        public int StatusId { get; set; }

        // Navigations
        public Customer Customer { get; set; }
        public Payment Payment { get; set; }
        public Shipment Shipment { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
