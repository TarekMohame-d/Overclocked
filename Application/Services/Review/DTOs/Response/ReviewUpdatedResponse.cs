namespace Application.Services.Review.DTOs.Response;

public record ReviewUpdatedResponse
{
    public required int Rating { get; init; }
    public required string Comment { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public double AverageRating { get; init; }
    public int ReviewCount { get; init; }
}
