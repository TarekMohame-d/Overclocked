using Domain.Entities.Common;
using Domain.StaticData;

namespace Domain.Entities;

public class Invoice : BaseEntity
{
    public Guid EmployeeId { get; set; }
    public int StatusId { get; set; }
    public InvoiceStatusType InvoiceStatusType
    {
        get => (InvoiceStatusType)StatusId;
        set => StatusId = (int)value;
    }
    public required string CustomerName { get; set; }
    public required string CustomerPhone { get; set; }
    public decimal TotalPrice { get; set; }

    // Navigation Properties
    public Employee? Employee { get; set; }
    public ICollection<InvoiceItem>? InvoiceItems { get; set; }
    public InvoiceStatus? InvoiceStatus { get; set; }
    public ICollection<Refund>? Refunds { get; set; }
}
