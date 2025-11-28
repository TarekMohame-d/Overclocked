namespace Application.Services.Review.DTOs.Response;

public record ReviewDeletedResponse
{
    public double AverageRating { get; init; }
    public int ReviewCount { get; init; }
}
