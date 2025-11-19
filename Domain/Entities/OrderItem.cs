using Domain.Entities.Common;

namespace Domain.Entities;

public class OrderItem : Entity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public Guid? ShipmentId { get; set; }
    public bool Shipped { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Navigation Properties
    public Order? Order { get; set; }
    public Product? Product { get; set; }
    public Shipment? Shipment { get; set; }
}
