namespace Overclocked.Contracts.Review;

public record RatingBreakdownResponse
{
    public required IDictionary<int, int> Ratings { get; init; } = new Dictionary<int, int>();
}
