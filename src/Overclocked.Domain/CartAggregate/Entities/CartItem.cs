using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.CartAggregate.Entities;

public sealed class CartItem : Entity<CartItemId>
{
    public ProductId ProductId { get; private set; } = null!;
    public int Quantity { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }

    private CartItem() { }

    private CartItem(CartItemId id, ProductId productId, int quantity)
        : base(id)
    {
        ProductId = productId;
        Quantity = quantity;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public static Result<CartItem> Create(ProductId productId, int quantity)
    {
        if (quantity <= 0)
            return Result.Failure<CartItem>(CartErrors.InvalidCartItemQuantity);

        var cartItem = new CartItem(CartItemId.Create(), productId, quantity);

        return Result.Success(cartItem);
    }

    internal Result UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            return Result.Failure<CartItem>(CartErrors.InvalidCartItemQuantity);

        Quantity = quantity;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }
}
