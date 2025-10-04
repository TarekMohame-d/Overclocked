namespace Domain.Entities;

public class Cart
{
    public Guid Id { get; protected set; }
    public Guid UserId { get; set; }

    // Navigation Properties
    public User? User { get; set; }
    public ICollection<CartItem>? CartItems { get; set; }

    public Cart()
    {
        Id = Guid.CreateVersion7();
    }
}

