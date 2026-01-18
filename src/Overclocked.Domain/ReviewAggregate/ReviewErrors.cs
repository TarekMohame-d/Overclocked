using Overclocked.SharedKernel;

namespace Overclocked.Domain.ReviewAggregate;

public static class ReviewErrors
{
    public static Error AlreadyReviewedThisProduct = Error.Conflict(
        "Review.AlreadyReviewedThisProduct",
        "You have already reviewed this product."
    );

    public static Error ReviewNotFound(Guid id) =>
        Error.NotFound("Review.NotFound", $"The Review with Id: '{id}' was not found.");

    public static Error ReviewAlreadyHasReply => Error.Conflict("Review.AlreadyHasReply", "Review already has reply.");

    public static Error ReviewReplyNotFound(Guid id) =>
        Error.NotFound("ReviewReply.NotFound", $"Review reply with Id: '{id}' was not found.");

    public static readonly Error UnauthorizedReplyUpdate = new(
        "ReviewReply.Unauthorized",
        "You are not authorized to update this reply.",
        ErrorType.Unauthorized
    );

    public static readonly Error UnauthorizedReplyDelete = new(
        "ReviewReply.Unauthorized",
        "You are not authorized to delete this reply.",
        ErrorType.Unauthorized
    );

    public static Error ReviewReplyIsRequired => Error.Validation("ReviewReply.Reply", "Reply is required.");

    public static Error ReviewReplyTooLong => Error.Validation("ReviewReply.Reply", "Reply must be less than 500 characters.");

    public static Error ReviewCommentIsRequired => Error.Validation("Review.Comment", "Comment is required.");

    public static Error ReviewCommentTooLong => Error.Validation("Review.Comment", "Comment must be less than 500 characters.");

    public static Error InvalidReviewRating => Error.Validation("Review.Rating", "Rating must be between 1 and 5.");
}
