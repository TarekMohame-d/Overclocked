namespace Application.Services.Review.DTOs.Response;

public record RatingBreakdownResponse
{
    public required IDictionary<int, int> Ratings { get; init; } = new Dictionary<int, int>();
}
