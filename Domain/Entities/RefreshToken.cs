using Domain.Entities.Common;

namespace Domain.Entities;

public class RefreshToken : Entity
{
    public Guid UserId { get; set; }
    public required string DeviceId { get; set; }
    public required string TokenHash { get; set; }
    public required DateTime ExpiredAt { get; set; }

    // Navigation properties
    public User? User { get; set; }
}
