namespace Application.Common.Results.PredefinedErrors;

public static partial class Errors
{
    public static readonly Error ReviewAlreadyHasReply = new(
        nameof(ReviewAlreadyHasReply),
        ErrorType.Conflict,
        "Review already has reply.");

    public static readonly Error ReviewReplyNotFound = new(
        nameof(ReviewReplyNotFound),
        ErrorType.NotFound,
        "Review reply not found.");
}
