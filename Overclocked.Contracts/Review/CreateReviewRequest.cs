namespace Overclocked.Contracts.Review;

public record CreateReviewRequest
{
    public required int Rating { get; init; }
    public required string Comment { get; init; }
}
