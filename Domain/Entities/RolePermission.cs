namespace Domain.Entities;

public class RolePermission
{
    public required int RoleId { get; set; }
    public required int PermissionId { get; set; }

    // Navigation Properties
    public Role? Role { get; set; }
    public Permission? Permission { get; set; }
}
