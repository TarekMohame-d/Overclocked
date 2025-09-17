namespace Domain.Entities;

public class EmailConfirmationCode
{
    public Guid UserId { get; set; }
    public required string CodeHash { get; set; }
    public bool IsUsed { get; set; }
    public DateTime ExpiredAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation Properties
    public User? User { get; set; }
}
