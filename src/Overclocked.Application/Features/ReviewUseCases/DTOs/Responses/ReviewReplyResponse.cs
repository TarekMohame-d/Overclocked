namespace Overclocked.Application.Features.ReviewUseCases.DTOs.Responses;

public record ReviewReplyResponse
{
    public required Guid Id { get; init; }
    public required string Reply { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
}
