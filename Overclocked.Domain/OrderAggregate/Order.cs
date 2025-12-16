using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.Common.Shared.ValueObjects;
using Overclocked.Domain.OrderAggregate.Entities;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Domain.OrderAggregate;

public class Order : AggregateRoot<OrderId>
{
    public UserId UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public Address ShippingAddress { get; private set; }
    public Money TotalPrice { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();


    private Order() { }

    private Order(
        OrderId id,
        UserId userId,
        Address shippingAddress) : base(id)
    {
        UserId = userId;
        Status = OrderStatus.Pending;
        ShippingAddress = shippingAddress;
        RecalculateTotal();

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Order Create(
        UserId userId,
        Address shippingAddress)
    {
        return new(OrderId.Create(), userId, shippingAddress);
    }

    // Domain logic
    private void RecalculateTotal()
    {
        Money subTotal = _items.Aggregate(
            Money.Zero,
            (current, item) => current + item.CalculateTotal()
        );

        TotalPrice = subTotal;
        UpdatedAt = DateTime.UtcNow;
    }
}
