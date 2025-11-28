namespace Application.Services.Review.DTOs.Request;

public record CreateReviewRequestBody
{
    public required int Rating { get; init; }
    public required string Comment { get; init; }
}

public record CreateReviewRequest : CreateReviewRequestBody
{
    public required Guid ProductId { get; init; }
    public required Guid UserId { get; init; }

    public static CreateReviewRequest FromBody(CreateReviewRequestBody requestBody, Guid userId, Guid productId)
    {
        return new()
        {
            UserId = userId,
            ProductId = productId,
            Rating = requestBody.Rating,
            Comment = requestBody.Comment
        };
    }
}
