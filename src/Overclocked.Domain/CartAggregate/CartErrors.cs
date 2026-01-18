using Overclocked.SharedKernel;

namespace Overclocked.Domain.CartAggregate;

public static class CartErrors
{
    public static Error CartItemNotFound(Guid id) =>
        Error.NotFound("Cart.ItemNotFound", $"The Cart Item with ID: '{id}' was not found.");

    public static Error InvalidCartItemQuantity => Error.Validation("CartItem.Quantity", "Quantity must be greater than 0.");
}
