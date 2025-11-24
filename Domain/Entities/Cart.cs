using Domain.Exceptions;

namespace Domain.Entities;

public class Cart
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public required Guid UserId { get; init; }

    // Navigation Properties
    public User? User { get; set; }
    public ICollection<CartItem> CartItems { get; set; } = [];

    public void AddOrUpdateItem(Guid productId, int quantity, int stockQuantity)
    {
        CartItem? existingItem = CartItems.SingleOrDefault(ci => ci.ProductId == productId);

        if(quantity > stockQuantity)
            throw new InvalidCartItemQuantityException(productId, quantity, stockQuantity);

        if(existingItem is not null)
        {
            existingItem.Quantity = quantity;
        }
        else
        {
            CartItems.Add(
                new CartItem
                {
                    CartId = Id,
                    ProductId = productId,
                    Quantity = quantity,
                });
        }
    }
}
