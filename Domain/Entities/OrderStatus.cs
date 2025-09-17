namespace Domain.Entities;

public class OrderStatus
{
    // Properties
    public int Id { get; set; }
    public required string Name { get; set; }

    // Navigation Properties
    public ICollection<Order>? Orders { get; set; }
}
