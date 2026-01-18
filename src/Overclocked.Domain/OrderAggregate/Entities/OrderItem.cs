using Overclocked.Domain.Common.Shared.ValueObjects.Image;
using Overclocked.Domain.Common.Shared.ValueObjects.Money;
using Overclocked.Domain.OrderAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.OrderAggregate.Entities;

public sealed class OrderItem : Entity<OrderItemId>
{
    private OrderItem() { }

    private OrderItem(OrderItemId id, ProductId productId, string productName, Image productImage, Money unitPrice, int quantity)
        : base(id)
    {
        ProductId = productId;
        ProductName = productName;
        ProductImage = productImage;
        UnitPrice = unitPrice;
        Quantity = quantity;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public ProductId ProductId { get; private set; } = null!;
    public string ProductName { get; private set; } = null!;
    public Image ProductImage { get; private set; } = null!;
    public Money UnitPrice { get; } = null!;
    public int Quantity { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Result<OrderItem> Create(
        ProductId productId,
        string productName,
        Image productImage,
        decimal unitPrice,
        int quantity
    )
    {
        if (string.IsNullOrWhiteSpace(productName) || productName.Length > 50)
            return Result.Failure<OrderItem>(OrderErrors.OrderItemInvalidProductName);

        if (unitPrice <= 0)
            return Result.Failure<OrderItem>(OrderErrors.OrderItemInvalidUnitPrice);

        if (quantity <= 0)
            return Result.Failure<OrderItem>(OrderErrors.OrderItemInvalidQuantity);

        Result<Money> unitPriceResult = Money.Create(unitPrice);

        if (unitPriceResult.IsFailure)
            return Result.Failure<OrderItem>(unitPriceResult.Error);

        var orderItem = new OrderItem(
            OrderItemId.Create(),
            productId,
            productName,
            productImage,
            unitPriceResult.Value,
            quantity
        );

        return Result.Success(orderItem);
    }

    internal Result UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            return Result.Failure<OrderItem>(OrderErrors.OrderItemInvalidQuantity);

        Quantity = quantity;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public Money CalculateLineTotalPrice() => UnitPrice * Quantity;
}
