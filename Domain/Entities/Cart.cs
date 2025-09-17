namespace Domain.Entities;

public class Cart
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public User? User { get; set; }
    public ICollection<CartItem>? CartItems { get; set; }
}

