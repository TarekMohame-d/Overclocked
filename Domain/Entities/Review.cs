using Domain.Entities.Common;

namespace Domain.Entities;

public class Review : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public required string Comment { get; set; }
    public int Rating { get; set; }

    // Navigation Properties
    public User User { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public ReviewReply ReviewReply { get; set; } = null!;
}
