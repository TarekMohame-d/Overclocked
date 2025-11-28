namespace Application.Services.ReviewReply.DTOs.Request;

public record DeleteReviewReplyRequest
{
    public required Guid ProductId { get; init; }
    public required Guid ReviewId { get; init; }
    public required Guid ReplyId { get; init; }
}
