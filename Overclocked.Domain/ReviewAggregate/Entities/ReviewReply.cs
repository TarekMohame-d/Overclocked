using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Domain.ReviewAggregate.Entities;

public class ReviewReply : Entity<ReviewReplyId>
{
    public UserId EmployeeId { get; private set; }
    public string Reply { get; private set; }
    public DateTime CreatedAt { get; private init; }
    public DateTime UpdatedAt { get; private set; }

    private ReviewReply()
    {
    }
    private ReviewReply(ReviewReplyId id, UserId employeeId, string reply) : base(id)
    {
        EmployeeId = employeeId;
        Reply = reply;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static ReviewReply Create(ReviewReplyId id, UserId employeeId, string reply)
    {
        return new(id, employeeId, reply);
    }

    public void Update(string reply)
    {
        Reply = reply;
        UpdatedAt = DateTime.UtcNow;
    }
}
