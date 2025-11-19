namespace Domain.Entities;

public class ShipmentStatus
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // Navigation Properties
    public ICollection<Shipment>? Shipments { get; set; }
}
