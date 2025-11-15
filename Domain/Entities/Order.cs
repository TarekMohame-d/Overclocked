using Domain.Entities.Common;
using Domain.StaticData;

namespace Domain.Entities;

public class Order : Entity
{
    public Guid UserId { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal TotalPrice { get; set; }
    public int StatusId { get; set; }
    public OrderStatusType OrderStatusType
    {
        get => (OrderStatusType)StatusId;
        set => StatusId = (int)value;
    }

    // Navigation Properties
    public User? User { get; set; }
    public Payment? Payment { get; set; }
    public OrderStatus? OrderStatus { get; set; }
    public ICollection<OrderItem>? OrderItems { get; set; }
    public ICollection<Shipment>? Shipments { get; set; }
    public Refund? Refund { get; set; }
}
