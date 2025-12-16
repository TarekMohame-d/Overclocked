namespace Overclocked.Domain.UserAggregate.Enums;

public enum Role
{
    SuperAdmin = 1, // Full system access
    Admin, // Manages platform sections
    DataEntry, // Manages data entry
    Customer, // Customer account
}
