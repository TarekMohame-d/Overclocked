using Domain.Entities.Common;

namespace Domain.Entities;

public class InvoiceItem : BaseEntity
{
    public Guid InvoiceId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // Navigation Properties
    public Invoice? Invoice { get; set; }
    public Product? Product { get; set; }
}
