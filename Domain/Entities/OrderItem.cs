using Domain.Entities.Common;

namespace Domain.Entities;

public class OrderItem : Entity
{
    public required Guid OrderId { get; set; }
    public required Guid ProductId { get; set; }
    public Guid? ShipmentId { get; set; }
    public bool Shipped { get; set; } = false;
    public required int Quantity { get; set; }
    public required decimal UnitPrice { get; set; }

    // Navigation Properties
    public Order? Order { get; set; }
    public Product? Product { get; set; }
    public Shipment? Shipment { get; set; }
}
