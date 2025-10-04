using Domain.Entities.Common;
using Domain.StaticData;

namespace Domain.Entities;

public class Refund : BaseEntity
{
    public Guid? OrderId { get; set; }
    public int StatusId { get; set; }
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
    public ICollection<RefundItem>? RefundItems { get; set; }
}
