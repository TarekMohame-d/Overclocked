using Domain.Entities.Common;

namespace Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid? UserId { get; set; }
    public Guid? EmployeeId { get; set; }
    public required string DeviceId { get; set; }
    public required string DeviceType { get; set; }
    public required string TokenHash { get; set; }
    public DateTime ExpiredAt { get; set; }

    // Navigation properties
    public User? User { get; set; }
    public Employee? Employee { get; set; }
}
