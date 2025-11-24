using Domain.Entities.Common;

namespace Domain.Entities;

public class EmailConfirmationCode : Entity
{
    public required Guid UserId { get; set; }
    public required string CodeHash { get; set; }
    public bool IsUsed { get; set; } = false;
    public required DateTime ExpiredAt { get; set; }

    // Navigation Properties
    public User? User { get; set; }
}
