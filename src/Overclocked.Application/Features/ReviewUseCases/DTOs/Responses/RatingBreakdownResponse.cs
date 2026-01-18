namespace Overclocked.Application.Features.ReviewUseCases.DTOs.Responses;

public record RatingBreakdownResponse
{
    public required IDictionary<int, int> Ratings { get; init; } = new Dictionary<int, int>();
}
