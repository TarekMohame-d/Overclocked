namespace Domain.Entities;

public class EmployeeActivityLog
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public required string Action { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public Employee? Employee { get; set; }
}
