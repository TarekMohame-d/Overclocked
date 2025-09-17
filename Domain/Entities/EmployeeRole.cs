namespace Domain.Entities;

public class EmployeeRole
{
    public int Id { get; set; }
    public required string Name { get; set; }

    // Navigation properties
    public ICollection<Employee>? Employees { get; set; }
    public ICollection<RolePermission>? RolePermissions { get; set; }
}
