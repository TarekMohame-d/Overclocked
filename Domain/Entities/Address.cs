using Domain.Entities.Common;

namespace Domain.Entities;

public class Address : Entity
{
    public required Guid UserId { get; init; }
    public required string City { get; set; }
    public required string Street { get; set; }
    public required string Description { get; set; }
    public bool IsDeleted { get; set; } = false;

    // Navigation Properties
    public User? User { get; set; }
    public ICollection<Shipment> Shipments { get; set; } = [];
}
