namespace Overclocked.Application.Features.ReviewUseCases.DTOs.Responses;

public record ReviewResponse
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required string UserEmail { get; init; }
    public required string UserName { get; init; }
    public required int Rating { get; init; }
    public required string Comment { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public ReviewReplyResponse? Reply { get; init; }
}
