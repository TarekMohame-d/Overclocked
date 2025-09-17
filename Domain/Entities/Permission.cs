namespace Domain.Entities;

public class Permission
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // Navigation Properties
    public ICollection<RolePermission>? RolePermissions { get; set; }
}
