using Domain.Entities.Common;

namespace Domain.Entities;

public class ReviewReply : Entity
{
    public required Guid ReviewId { get; set; }
    public required Guid EmployeeId { get; set; }
    public required string Reply { get; set; }

    // Navigation Properties
    public User? Employee { get; set; }
    public Review? Review { get; set; }
}
