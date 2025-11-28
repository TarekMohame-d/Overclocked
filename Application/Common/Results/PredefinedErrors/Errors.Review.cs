namespace Application.Common.Results.PredefinedErrors;

public static partial class Errors
{
    public static readonly Error AlreadyReviewedThisProduct = new(
        nameof(AlreadyReviewedThisProduct),
        ErrorType.Conflict,
        "You have already reviewed this product.");

    public static readonly Error ReviewNotFound = new(
        nameof(ReviewNotFound),
        ErrorType.NotFound,
        "The specified review was not found. It may have been deleted.");
}
