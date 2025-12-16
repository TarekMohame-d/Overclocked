using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Domain.CartAggregate.Entities;

public class CartItem : Entity<CartItemId>
{
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime CreatedAt { get; private init; }
    public DateTime UpdatedAt { get; private set; }

    private CartItem()
    {
    }
    private CartItem(CartItemId id, ProductId productId, int quantity) : base(id)
    {
        ProductId = productId;
        Quantity = quantity;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static CartItem Create(ProductId productId, int quantity)
    {
        return new(
            id: CartItemId.Create(),
            productId: productId,
            quantity: quantity);
    }

    internal void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
