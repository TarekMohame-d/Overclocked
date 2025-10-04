namespace Domain.Entities;

public class Role
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // Navigation properties
    public ICollection<User>? Users { get; set; }
    public ICollection<RolePermission>? RolePermissions { get; set; }
}
