namespace Application.Services.Review.DTOs.Response;

public record ReviewCreatedResponse
{
    public required int Rating { get; init; }
    public required string Comment { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required double AverageRating { get; init; }
    public required int ReviewCount { get; init; }
}
