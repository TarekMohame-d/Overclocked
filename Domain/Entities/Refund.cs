using Domain.Entities.Common;
using Domain.StaticData;

namespace Domain.Entities;

public class Refund : Entity
{
    public required Guid OrderId { get; set; }
    public int StatusId { get; private set; }
    public RefundStatusType RefundStatusType
    {
        get => (RefundStatusType)StatusId;
        set => StatusId = (int)value;
    }
    public decimal RefundAmount { get; set; }
    public required string RefundReason { get; set; }

    // Navigation properties
    public Order? Order { get; set; }
    public RefundStatus? RefundStatus { get; set; }
    public ICollection<RefundItem> RefundItems { get; set; } = [];
}
