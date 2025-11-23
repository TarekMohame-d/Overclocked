using Domain.Entities.Common;

namespace Domain.Entities;

public class RefundItem : Entity
{
    public required Guid RefundId { get; set; }
    public required Guid OrderItemId { get; set; }
    public required int Quantity { get; set; }

    // Navigation properties
    public Refund? Refund { get; set; }
    public OrderItem? OrderItem { get; set; }
}
