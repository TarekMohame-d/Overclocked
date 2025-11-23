namespace Domain.Entities;

public class EmployeeActivityLog
{
    public Guid Id { get; } = Guid.CreateVersion7();
    public required Guid EmployeeId { get; set; }
    public required string Action { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public User? Employee { get; set; }
}
