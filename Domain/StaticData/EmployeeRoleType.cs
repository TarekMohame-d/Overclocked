namespace Domain.StaticData;

public enum EmployeeRoleType
{
    SuperAdmin = 1, // Full system access
    Admin,          // Manages platform sections
    DataEntry,      // Manages data entry
    Manager,        // Handles teams or business units
    Employee        // Internal system user with limited rights
}
