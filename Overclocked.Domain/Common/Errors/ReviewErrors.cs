using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Domain.Common.Errors;

public static class ReviewErrors
{
    public static readonly Error AlreadyReviewedThisProduct = new(
        "Review.AlreadyReviewedThisProduct",
        "You have already reviewed this product.",
        ErrorType.Conflict);

    public static Error ReviewNotFound(Guid id) => new(
        "Review.NotFound",
        $"The Review with Id: '{id}' was not found.",
        ErrorType.NotFound);

    public static readonly Error ReviewAlreadyHasReply = new(
        "Review.AlreadyHasReply",
        "Review already has reply.",
        ErrorType.Conflict);

    public static Error ReviewReplyNotFound(Guid id) => new(
        "ReviewReply.NotFound",
        $"Review reply with Id: '{id}' was not found.",
        ErrorType.NotFound);

    public static readonly Error UnauthorizedReplyUpdate = new(
        "ReviewReply.Unauthorized",
        "You are not authorized to update this reply.",
        ErrorType.Unauthorized);

    public static readonly Error UnauthorizedReplyDelete = new(
        "ReviewReply.Unauthorized",
        "You are not authorized to delete this reply.",
        ErrorType.Unauthorized);
}
