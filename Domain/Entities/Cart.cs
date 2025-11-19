using Domain.Exceptions;

namespace Domain.Entities;

public class Cart
{
    public Guid Id { get; protected set; }
    public Guid UserId { get; set; }

    // Navigation Properties
    public User? User { get; set; }
    public ICollection<CartItem> CartItems { get; private set; }

    public Cart()
    {
        Id = Guid.CreateVersion7();
        CartItems = [];
    }

    public void AddOrUpdateItem(Guid productId, int quantity, int stockQuantity)
    {
        CartItem? existingItem = CartItems.SingleOrDefault(ci => ci.ProductId == productId);

        if(quantity > stockQuantity)
        {
            throw new InvalidCartItemQuantityException(productId, quantity, stockQuantity);
        }

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
                }
            );
        }
    }

    public void RemoveItem(Guid productId)
    {
        CartItem? item = CartItems.SingleOrDefault(ci => ci.ProductId == productId);
        if(item != null)
            CartItems.Remove(item);
    }

    public void CLearCart() => CartItems.Clear();
}
