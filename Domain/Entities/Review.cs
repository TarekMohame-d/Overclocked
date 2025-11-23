using Domain.Entities.Common;

namespace Domain.Entities;

public class Review : Entity
{
    public required Guid UserId { get; set; }
    public required Guid ProductId { get; set; }
    public required string Comment { get; set; }
    public required int Rating { get; set; }

    // Navigation Properties
    public User? User { get; set; }
    public Product? Product { get; set; }
    public ReviewReply? ReviewReply { get; set; }
}
