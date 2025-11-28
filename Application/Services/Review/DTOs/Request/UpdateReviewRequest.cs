namespace Application.Services.Review.DTOs.Request;

public record UpdateReviewRequestBody
{
    public required int Rating { get; init; }
    public required string Comment { get; init; }
}

public record UpdateReviewRequest : UpdateReviewRequestBody
{
    public required Guid ProductId { get; init; }
    public required Guid UserId { get; init; }
    public required Guid ReviewId { get; init; }

    public static UpdateReviewRequest FromBody(
        UpdateReviewRequestBody requestBody,
        Guid userId,
        Guid productId,
        Guid reviewId)
    {
        return new()
        {
            ReviewId = reviewId,
            UserId = userId,
            ProductId = productId,
            Rating = requestBody.Rating,
            Comment = requestBody.Comment
        };
    }
}
