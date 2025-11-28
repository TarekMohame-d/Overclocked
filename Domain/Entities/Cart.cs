using Domain.Exceptions;

namespace Domain.Entities;

public class Cart
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public required Guid UserId { get; init; }

    // Navigation Properties
    public User? User { get; set; }
    public ICollection<CartItem> CartItems { get; set; } = [];

    public void AddCartItem(Guid productId, int quantity, int stockQuantity)
    {
        CartItem? existingItem = CartItems.FirstOrDefault(x => x.ProductId == productId);

        if(existingItem is not null)
        {
            // Update existing
            var newQuantity = existingItem.Quantity + quantity;
            if(newQuantity > stockQuantity)
                throw new InvalidCartItemQuantityException(productId, newQuantity, stockQuantity);

            existingItem.Quantity = newQuantity;
            existingItem.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            // Add new
            if(quantity > stockQuantity)
                throw new InvalidCartItemQuantityException(productId, quantity, stockQuantity);

            CartItems.Add(new CartItem
            {
                CartId = Id,
                ProductId = productId,
                Quantity = quantity,
            });
        }
    }

    public void UpdateItem(Guid itemId, int quantity, int stockQuantity)
    {
        CartItem existingItem = CartItems.Single(ci => ci.Id == itemId);

        if(quantity > stockQuantity)
            throw new InvalidCartItemQuantityException(existingItem.ProductId, quantity, stockQuantity);

        existingItem.Quantity = quantity;
        existingItem.UpdatedAt = DateTime.UtcNow;
    }
}
