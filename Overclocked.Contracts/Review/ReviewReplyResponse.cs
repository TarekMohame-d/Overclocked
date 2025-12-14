namespace Overclocked.Contracts.Review;

public record ReviewReplyResponse
{
    public required Guid Id { get; init; }
    public required string Reply { get; init; }
    public required DateTime CreatedAt { get; init; }
}
