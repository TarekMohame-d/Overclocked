using Overclocked.Domain.CartAggregate.Entities;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Domain.CartAggregate;

public class Cart : AggregateRoot<CartId>
{
    public UserId UserId { get; private set; }

    private readonly List<CartItem> _cartItems = [];
    public IReadOnlyList<CartItem> CartItems => _cartItems.AsReadOnly();

    private Cart()
    {
    }
    private Cart(CartId id, UserId userId) : base(id)
    {
        UserId = userId;
    }

    public static Cart Create(CartId id, UserId userId)
    {
        return new(id, userId);
    }

    public void AddCartItem(ProductId productId, int quantity)
    {
        CartItem? existingItem = _cartItems.FirstOrDefault(i => i.ProductId == productId);

        if(existingItem is not null)
        {
            existingItem.AddQuantity(quantity);
        }
        else
        {
            var cartItem = CartItem.Create(CartItemId.Create(), Id, productId, quantity);
            _cartItems.Add(cartItem);
        }
    }

    public void RemoveCartItem(ProductId productId)
    {
        CartItem? item = _cartItems.FirstOrDefault(i => i.ProductId == productId);
        if(item is not null)
        {
            _cartItems.Remove(item);
        }
    }
}
