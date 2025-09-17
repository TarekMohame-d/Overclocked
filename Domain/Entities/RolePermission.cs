namespace Domain.Entities;

public class RolePermission
{
    public int RoleId { get; set; }
    public int PermissionId { get; set; }

    // Navigation Properties
    public EmployeeRole? EmployeeRole { get; set; }
    public Permission? Permission { get; set; }
}
