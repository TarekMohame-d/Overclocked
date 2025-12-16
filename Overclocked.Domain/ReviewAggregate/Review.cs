using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate.Entities;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Domain.ReviewAggregate;

public class Review : AggregateRoot<ReviewId>
{
    public UserId UserId { get; private set; }
    public ProductId ProductId { get; private set; }
    public string Comment { get; private set; }
    public int Rating { get; private set; }
    public DateTime CreatedAt { get; private init; }
    public DateTime UpdatedAt { get; private set; }
    public ReviewReply? ReviewReply { get; private set; }

    public User User { get; }

    private Review()
    {
    }
    private Review(
        ReviewId id,
        UserId userId,
        ProductId productId,
        string comment,
        int rating,
        ReviewReply? reviewReply = null) : base(id)
    {
        UserId = userId;
        ProductId = productId;
        ReviewReply = reviewReply;
        Comment = comment;
        Rating = rating;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Review Create(
        UserId userId,
        ProductId productId,
        string comment,
        int rating)
    {
        return new(ReviewId.Create(), userId, productId, comment, rating);
    }

    public void Update(string comment, int rating)
    {
        Comment = comment;
        Rating = rating;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddReviewReply(ReviewReply reviewReply)
    {
        ReviewReply = reviewReply;
        UpdatedAt = DateTime.UtcNow;
    }

    public Result UpdateReply(ReviewReplyId replyId, UserId employeeId, string newReply)
    {
        if(ReviewReply is null || ReviewReply.Id != replyId)
        {
            return Result.Failure(ReviewErrors.ReviewReplyNotFound(replyId.Value));
        }

        if(ReviewReply.EmployeeId != employeeId)
        {
            return Result.Failure(ReviewErrors.UnauthorizedReplyUpdate);
        }

        ReviewReply.Update(newReply);

        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }

    public Result DeleteReply(ReviewReplyId replyId, UserId employeeId)
    {
        if(ReviewReply is null || ReviewReply.Id != replyId)
        {
            return Result.Failure(ReviewErrors.ReviewReplyNotFound(replyId.Value));
        }

        if(ReviewReply.EmployeeId != employeeId)
        {
            return Result.Failure(ReviewErrors.UnauthorizedReplyDelete);
        }

        ReviewReply = null;
        UpdatedAt = DateTime.UtcNow;

        return Result.Success();
    }
}
