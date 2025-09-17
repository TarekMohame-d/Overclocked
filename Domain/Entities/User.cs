using Domain.Entities.Common;

namespace Domain.Entities;

public class User : BaseEntity
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public required string PasswordHash { get; set; }
    public required string Phone { get; set; }

    // Navigation properties
    public ICollection<Address>? Addresses { get; set; }
    public ICollection<Order>? Orders { get; set; }
    public ICollection<RefreshToken>? RefreshTokens { get; set; }
    public EmailConfirmationCode? EmailConfirmationCode { get; set; }
    public Cart? Cart { get; set; }
    public Wishlist? Wishlist { get; set; }
    public ICollection<Review>? Reviews { get; set; }
    public ICollection<Shipment>? Shipments { get; set; }
    public ICollection<Payment>? Payments { get; set; }
}
