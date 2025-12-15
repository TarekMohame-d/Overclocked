namespace Overclocked.Contracts.ReviewReply;

public record UpdateReviewReplyRequest
{
    public required string Reply { get; init; }
}
