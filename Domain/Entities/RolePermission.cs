namespace Domain.Entities;

public class RolePermission
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    // Navigation Properties
    public Role? Role { get; set; }
    public Permission? Permission { get; set; }
}
