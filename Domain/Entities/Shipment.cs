using Domain.Entities.Common;
using Domain.StaticData;

namespace Domain.Entities;

public class Shipment : Entity
{
    public Guid OrderId { get; set; }
    public Guid AddressId { get; set; }
    public int StatusId { get; set; }
    public ShipmentStatusType ShipmentStatusType
    {
        get => (ShipmentStatusType)StatusId;
        set => StatusId = (int)value;
    }
    public required string CarrierName { get; set; }
    public required string TrackingNumber { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime EstimatedDeliveryDate { get; set; }
    public DateTime? DeliveredAt { get; set; }

    // Navigation Properties
    public Address? Address { get; set; }
    public Order? Order { get; set; }
    public ShipmentStatus? ShipmentStatus { get; set; }
    public ICollection<OrderItem>? ShipmentItems { get; set; }
}
