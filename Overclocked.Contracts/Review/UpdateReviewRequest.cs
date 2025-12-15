namespace Overclocked.Contracts.Review;

public record UpdateReviewRequest
{
    public required int Rating { get; init; }
    public required string Comment { get; init; }
}
