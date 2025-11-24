namespace Domain.Entities;

public class RefundStatus
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    // Navigation Properties
    public ICollection<Refund> Refunds { get; set; } = [];
}
