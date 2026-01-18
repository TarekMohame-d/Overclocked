using Overclocked.Domain.CartAggregate.Entities;
using Overclocked.Domain.CartAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.CartAggregate;

public sealed class Cart : AggregateRoot<CartId>
{
    public UserId UserId { get; private set; } = null!;

    private readonly List<CartItem> _cartItems = [];
    public IReadOnlyList<CartItem> CartItems => _cartItems.AsReadOnly();

    private Cart() { }

    private Cart(CartId id, UserId userId)
        : base(id) => UserId = userId;

    public static Cart Create(UserId userId) => new(CartId.Create(), userId);

    public Result<CartItemId> AddCartItem(ProductId productId, int quantity)
    {
        CartItem? existingItem = _cartItems.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem is not null)
        {
            Result result = existingItem.UpdateQuantity(quantity);

            if (result.IsFailure)
                return Result.Failure<CartItemId>(result.Error);

            return Result.Success(existingItem.Id);
        }

        Result<CartItem> cartItemResult = CartItem.Create(productId, quantity);

        if (cartItemResult.IsFailure)
            return Result.Failure<CartItemId>(cartItemResult.Error);

        CartItem cartItem = cartItemResult.Value;
        _cartItems.Add(cartItem);

        return Result.Success(cartItem.Id);
    }

    public Result UpdateCartItem(CartItemId cartItemId, int quantity)
    {
        CartItem? existingItem = _cartItems.FirstOrDefault(ci => ci.Id == cartItemId);

        if (existingItem is null)
            return Result.Failure(CartErrors.CartItemNotFound(cartItemId.Value));

        Result result = existingItem.UpdateQuantity(quantity);

        if (result.IsFailure)
            return Result.Failure<CartItemId>(result.Error);

        return Result.Success();
    }

    public void RemoveCartItem(CartItemId cartItemId)
    {
        CartItem? item = _cartItems.FirstOrDefault(ci => ci.Id == cartItemId);
        if (item is null)
            return;

        _cartItems.Remove(item);
    }

    public void Clear() => _cartItems.Clear();
}
