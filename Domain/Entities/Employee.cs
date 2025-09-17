using Domain.Entities.Common;
using Domain.StaticData;

namespace Domain.Entities;

public class Employee : BaseEntity
{
    public int RoleId { get; set; }
    public EmployeeRoleType RoleType
    {
        get => (EmployeeRoleType)RoleId;
        set => RoleId = (int)value;
    }
    public required string Username { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PasswordHash { get; set; }
    public required string Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }

    // Navigation Properties
    public ICollection<EmployeeActivityLog>? ActivityLogs { get; set; }
    public ICollection<RefreshToken>? RefreshTokens { get; set; }
    public ICollection<Invoice>? Invoices { get; set; }
    public EmployeeRole? Role { get; set; }
    public ICollection<ReviewReply>? ReviewReplies { get; set; }
    public ICollection<Refund>? Refunds { get; set; }
}
