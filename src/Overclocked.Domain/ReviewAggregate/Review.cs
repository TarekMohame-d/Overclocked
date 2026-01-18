using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate.Entities;
using Overclocked.Domain.ReviewAggregate.Events;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.SharedKernel;
using Overclocked.SharedKernel.Primitives;

namespace Overclocked.Domain.ReviewAggregate;

public sealed class Review : AggregateRoot<ReviewId>
{
    public UserId UserId { get; private set; } = null!;
    public ProductId ProductId { get; private set; } = null!;
    public string Comment { get; private set; } = null!;
    public int Rating { get; private set; }
    public DateTimeOffset CreatedAt { get; private init; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public ReviewReply? ReviewReply { get; private set; }
    public User? User { get; }

    private Review() { }

    private Review(ReviewId id, UserId userId, ProductId productId, string comment, int rating, ReviewReply? reviewReply = null)
        : base(id)
    {
        UserId = userId;
        ProductId = productId;
        ReviewReply = reviewReply;
        Comment = comment;
        Rating = rating;

        DateTimeOffset now = DateTimeOffset.UtcNow;
        CreatedAt = now;
        UpdatedAt = now;
    }

    public static Result<Review> Create(UserId userId, ProductId productId, string comment, int rating)
    {
        comment = comment.Trim();
        Result validationResult = ValidateState(comment, rating);
        if (validationResult.IsFailure)
            return Result.Failure<Review>(validationResult.Error);

        var review = new Review(ReviewId.Create(), userId, productId, comment, rating);

        review.RaiseDomainEvent(new ReviewCreatedEvent(productId.Value, rating));

        return Result.Success(review);
    }

    public Result Update(string comment, int rating)
    {
        comment = comment.Trim();

        Result validationResult = ValidateState(comment, rating);
        if (validationResult.IsFailure)
            return Result.Failure<Review>(validationResult.Error);

        RaiseDomainEvent(new ReviewUpdatedEvent(ProductId.Value, Rating, rating));

        Comment = comment;
        Rating = rating;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public void Delete(Guid productId, int rating) => RaiseDomainEvent(new ReviewDeletedEvent(productId, rating));

    public void AddReviewReply(ReviewReply reviewReply)
    {
        ReviewReply = reviewReply;
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public Result UpdateReply(ReviewReplyId replyId, UserId employeeId, string newReply)
    {
        if (ReviewReply is null || ReviewReply.Id != replyId)
            return Result.Failure(ReviewErrors.ReviewReplyNotFound(replyId.Value));

        if (ReviewReply.EmployeeId != employeeId)
            return Result.Failure(ReviewErrors.UnauthorizedReplyUpdate);

        Result result = ReviewReply.Update(newReply);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    public Result DeleteReply(ReviewReplyId replyId, UserId employeeId)
    {
        if (ReviewReply is null || ReviewReply.Id != replyId)
            return Result.Failure(ReviewErrors.ReviewReplyNotFound(replyId.Value));

        if (ReviewReply.EmployeeId != employeeId)
            return Result.Failure(ReviewErrors.UnauthorizedReplyDelete);

        ReviewReply = null;
        UpdatedAt = DateTimeOffset.UtcNow;

        return Result.Success();
    }

    private static Result ValidateState(string comment, int rating)
    {
        if (string.IsNullOrWhiteSpace(comment))
            return Result.Failure(ReviewErrors.ReviewCommentIsRequired);

        if (comment.Length > 500)
            return Result.Failure(ReviewErrors.ReviewCommentIsRequired);

        if (rating is < 1 or > 5)
            return Result.Failure(ReviewErrors.InvalidReviewRating);

        return Result.Success();
    }
}
