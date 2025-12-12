using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Domain.CartAggregate.Entities;

public class CartItem : Entity<CartItemId>
{
    public CartId CartId { get; private set; }
    public ProductId ProductId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime CreatedAt { get; private init; }
    public DateTime UpdatedAt { get; private set; }

    private CartItem()
    {
    }
    private CartItem(CartItemId id, CartId cartId, ProductId productId, int quantity) : base(id)
    {
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static CartItem Create(CartItemId id, CartId cartId, ProductId productId, int quantity)
    {
        return new(
            id: id,
            cartId: cartId,
            productId: productId,
            quantity: quantity);
    }

    internal void AddQuantity(int quantity)
    {
        Quantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}
