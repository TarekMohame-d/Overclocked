using Overclocked.Domain.Common.Shared.ValueObjects.Money;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.PaymentAggregate.Events;
using Overclocked.Domain.PaymentAggregate.ValueObjects;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.PaymentAggregate;

public sealed class Payment : AggregateRoot<PaymentId>
{
    public OrderId OrderId { get; private set; } = null!;
    public string PaymentProvider { get; private set; } = null!;
    public string PaymentMethod { get; private set; } = null!;
    public PaymentStatus Status { get; private set; }
    public Money Amount { get; private set; } = null!;
    public string? TransactionId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private Payment() { }

    private Payment(
        PaymentId id,
        OrderId orderId,
        string paymentProvider,
        string paymentMethod,
        Money totalPrice,
        string? transactionId = null
    )
        : base(id)
    {
        OrderId = orderId;
        PaymentProvider = paymentProvider;
        PaymentMethod = paymentMethod;
        Status = PaymentStatus.Pending;
        Amount = totalPrice;
        TransactionId = transactionId;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public static Payment Create(OrderId orderId, string paymentProvider, string paymentMethod, Money totalPrice) =>
        new(PaymentId.Create(), orderId, paymentProvider, paymentMethod, totalPrice);

    public void UpdatePaymentInfo(string paymentProvider, string paymentMethod)
    {
        PaymentProvider = paymentProvider;
        PaymentMethod = paymentMethod;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsPaid(string? transactionId = null)
    {
        Status = PaymentStatus.Paid;
        TransactionId = transactionId;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsFailed(Guid orderId)
    {
        Status = PaymentStatus.Failed;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new PaymentFailedEvent(orderId));
    }

    public void MarkAsRefunded()
    {
        Status = PaymentStatus.Refunded;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsCancelled()
    {
        Status = PaymentStatus.Cancelled;
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}
