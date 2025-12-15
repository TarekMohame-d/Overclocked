namespace Overclocked.Contracts.ReviewReply;

public record CreateReviewReplyRequest
{
    public required string Reply { get; init; }
}
