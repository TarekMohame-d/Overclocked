using Overclocked.Domain.CartAggregate.Entities;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.Common.Results;
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

    public static Cart Create(UserId userId)
    {
        return new(CartId.Create(), userId);
    }

    public CartItemId AddCartItem(ProductId productId, int quantity)
    {
        CartItem? existingItem = _cartItems.FirstOrDefault(i => i.ProductId == productId);

        if(existingItem is not null)
        {
            existingItem.UpdateQuantity(quantity);
            return existingItem.Id;
        }
        else
        {
            var cartItem = CartItem.Create(productId, quantity);
            _cartItems.Add(cartItem);
            return cartItem.Id;
        }
    }

    public Result UpdateCartItem(CartItemId cartItemId, int quantity)
    {
        CartItem? existingItem = _cartItems.FirstOrDefault(ci => ci.Id == cartItemId);

        if(existingItem is null)
        {
            return Result.Failure(CartErrors.CartItemNotFound(cartItemId.Value));
        }

        existingItem.UpdateQuantity(quantity);

        return Result.Success();
    }

    public void RemoveCartItem(CartItemId cartItemId)
    {
        CartItem? item = _cartItems.FirstOrDefault(ci => ci.Id == cartItemId);
        if(item is not null)
        {
            _cartItems.Remove(item);
        }
    }

    public void ClearCart()
    {
        _cartItems.Clear();
    }
}
