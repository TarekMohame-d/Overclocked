using Domain.Entities.Common;

namespace Domain.Entities;

public class RefundItem : BaseEntity
{
    public Guid RefundId { get; set; }
    public Guid? OrderItemId { get; set; }
    public Guid? InvoiceItemId { get; set; }
    public int Quantity { get; set; }

    // Navigation properties
    public Refund? Refund { get; set; }
    public OrderItem? OrderItem { get; set; }
}
