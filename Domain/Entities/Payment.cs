using Domain.Entities.Common;
using Domain.StaticData;

namespace Domain.Entities;

public class Payment : Entity
{
    public int StatusId { get; private set; }
    public PaymentStatusType PaymentStatusType
    {
        get => (PaymentStatusType)StatusId;
        set => StatusId = (int)value;
    }
    public required Guid OrderId { get; set; }
    public int MethodId { get; private set; }
    public PaymentMethodType PaymentMethodType
    {
        get => (PaymentMethodType)MethodId;
        set => MethodId = (int)value;
    }
    public string? TransactionId { get; set; }
    public required decimal Amount { get; set; }

    // Navigation Properties
    public PaymentMethod? PaymentMethod { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public Order? Order { get; set; }
}
