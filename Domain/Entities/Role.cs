namespace Domain.Entities;

public class Role
{
    public required int Id { get; set; }
    public required string Name { get; set; }

    // Navigation properties
    public ICollection<User> Users { get; set; } = [];
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
}
