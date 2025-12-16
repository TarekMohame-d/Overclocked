using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.Common.Shared.ValueObjects;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Domain.OrderAggregate.Entities;

public class OrderItem : Entity<OrderItemId>
{
    public ProductId ProductId { get; private set; }
    public string ProductName { get; private set; }

    public Money UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private OrderItem()
    {
    }

    private OrderItem(
        OrderItemId id,
        ProductId productId,
        string productName,
        Money unitPrice,
        int quantity) : base(id)
    {
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static OrderItem Create(
        ProductId productId,
        string productName,
        Money unitPrice,
        int quantity)
    {
        return new(OrderItemId.Create(), productId, productName, unitPrice, quantity);
    }

    public Money CalculateTotal() => UnitPrice * Quantity;
}
