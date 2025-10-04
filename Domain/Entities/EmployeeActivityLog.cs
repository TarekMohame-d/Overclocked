namespace Domain.Entities;

public class EmployeeActivityLog
{
    public Guid Id { get; protected set; }
    public Guid EmployeeId { get; set; }
    public required string Action { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation Properties
    public User? Employee { get; set; }

    public EmployeeActivityLog()
    {
        Id = Guid.CreateVersion7();
    }
}
