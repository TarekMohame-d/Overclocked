using System.Security.Cryptography;
using Overclocked.Domain.Common.Shared.ValueObjects.Address;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;
using Overclocked.Domain.Common.Shared.ValueObjects.Money;
using Overclocked.Domain.OrderAggregate.Entities;
using Overclocked.Domain.OrderAggregate.Events;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.OrderAggregate;

public sealed class Order : AggregateRoot<OrderId>
{
    private readonly List<OrderItem> _items = [];

    private Order() { }

    private Order(OrderId id, UserId userId, Address shippingAddress)
        : base(id)
    {
        OrderNumber = GenerateOrderNumber();
        UserId = userId;
        Status = OrderStatus.PendingPayment;
        ShippingAddress = shippingAddress;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public UserId UserId { get; private set; } = null!;
    public string OrderNumber { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public Address ShippingAddress { get; private set; } = null!;
    public Money TotalPrice { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    public static Order Create(UserId userId, Address shippingAddress) => new(OrderId.Create(), userId, shippingAddress);

    // Domain logic
    public Money CalculateTotalPrice()
    {
        TotalPrice = _items.Aggregate(Money.Zero, (current, item) => current + item.CalculateLineTotalPrice());

        return TotalPrice;
    }

    public Result AddItem(ProductId productId, string productName, Image productImage, decimal unitPrice, int quantity)
    {
        OrderItem? existingItem = _items.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem is not null)
        {
            existingItem.UpdateQuantity(quantity);
        }
        else
        {
            Result<OrderItem> orderItemResult = OrderItem.Create(productId, productName, productImage, unitPrice, quantity);

            if (orderItemResult.IsFailure)
                return Result.Failure(orderItemResult.Error);

            _items.Add(orderItemResult.Value);
        }

        return Result.Success();
    }

    public void UpdateShippingAddress(Address shippingAddress) => ShippingAddress = shippingAddress;

    public void MarkAsPlaced(bool isCod = false, bool isBalance = false)
    {
        Status = OrderStatus.Placed;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new OrderPlacedEvent(Id.Value, isCod, isBalance));
    }

    public void MarkAsProcessing()
    {
        Status = OrderStatus.Processing;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void MarkAsCancelled()
    {
        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new OrderCancelledEvent(Id.Value));
    }

    public void MarkAsRefunded(bool addToBalance)
    {
        Status = OrderStatus.Refunded;
        UpdatedAt = DateTimeOffset.UtcNow;

        RaiseDomainEvent(new OrderRefundedEvent(Id.Value, addToBalance));
    }

    private static string GenerateOrderNumber()
    {
        var segment1 = DateTime.UtcNow.ToString("yy")[1..] + DateTime.UtcNow.DayOfYear.ToString("D2");

        var segment2 = DateTime.UtcNow.ToString("HHmmss") + RandomNumberGenerator.GetInt32(0, 10).ToString();

        var segment3 = RandomNumberGenerator.GetInt32(1000000, 9999999).ToString();

        return $"{segment1}-{segment2}-{segment3}";
    }
}
