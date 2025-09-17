namespace Domain.Entities;

public class InvoiceStatus
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // Navigation Properties
    public ICollection<Invoice>? Invoices { get; set; }
}
