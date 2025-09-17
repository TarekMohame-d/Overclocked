using Domain.Entities.Common;
using Domain.StaticData;

namespace Domain.Entities;

public class Payment : BaseEntity
{
    public int StatusId { get; set; }
    public PaymentStatusType PaymentStatusType
    {
        get => (PaymentStatusType)StatusId;
        set => StatusId = (int)value;
    }
    public Guid UserId { get; set; }
    public Guid OrderId { get; set; }
    public int MethodId { get; set; }
    public PaymentMethodType PaymentMethodType
    {
        get => (PaymentMethodType)MethodId;
        set => MethodId = (int)value;
    }
    public string? TransactionId { get; set; }
    public decimal Amount { get; set; }

    // Navigation Properties
    public PaymentMethod? PaymentMethod { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public Order? Order { get; set; }
    public User? User { get; set; }
}
