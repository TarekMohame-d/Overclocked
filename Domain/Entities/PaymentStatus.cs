namespace Domain.Entities;

public class PaymentStatus
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // Navigation Properties
    public ICollection<Payment>? Payments { get; set; }
}
