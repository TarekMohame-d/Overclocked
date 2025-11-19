namespace Domain.StaticData;

public enum RoleType
{
    SuperAdmin = 1, // Full system access
    Admin, // Manages platform sections
    DataEntry, // Manages data entry
    Customer, // Customer account
}
