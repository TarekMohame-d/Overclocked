namespace Application.Services.Review.DTOs.Request;

public record DeleteReviewRequest
{
    public required Guid ReviewId { get; init; }
    public required Guid ProductId { get; init; }
    public required Guid UserId { get; init; }
}
