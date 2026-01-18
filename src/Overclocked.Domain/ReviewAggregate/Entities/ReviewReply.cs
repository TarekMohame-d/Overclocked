using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.ReviewAggregate.Entities;

public sealed class ReviewReply : Entity<ReviewReplyId>
{
    private ReviewReply() { }

    private ReviewReply(ReviewReplyId id, UserId employeeId, string reply)
        : base(id)
    {
        EmployeeId = employeeId;
        Reply = reply;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public UserId EmployeeId { get; private set; } = null!;
    public string Reply { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }

    public static Result<ReviewReply> Create(UserId employeeId, string reply)
    {
        reply = reply.Trim();
        if (string.IsNullOrWhiteSpace(reply))
            return Result.Failure<ReviewReply>(ReviewErrors.ReviewReplyIsRequired);

        if (reply.Length > 500)
            return Result.Failure<ReviewReply>(ReviewErrors.ReviewReplyTooLong);

        var reviewReply = new ReviewReply(ReviewReplyId.Create(), employeeId, reply);

        return Result.Success(reviewReply);
    }

    public Result Update(string reply)
    {
        reply = reply.Trim();
        if (string.IsNullOrWhiteSpace(reply))
            return Result.Failure<ReviewReply>(ReviewErrors.ReviewReplyIsRequired);

        if (reply.Length > 500)
            return Result.Failure<ReviewReply>(ReviewErrors.ReviewReplyTooLong);

        Reply = reply;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }
}
