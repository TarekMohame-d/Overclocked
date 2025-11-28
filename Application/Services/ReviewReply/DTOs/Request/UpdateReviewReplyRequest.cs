namespace Application.Services.ReviewReply.DTOs.Request;

public record UpdateReviewReplyRequestBody
{
    public required string Reply { get; init; }
}

public record UpdateReviewReplyRequest : UpdateReviewReplyRequestBody
{
    public required Guid ReviewId { get; init; }
    public required Guid EmployeeId { get; init; }
    public required Guid ProductId { get; init; }
    public required Guid ReplyId { get; init; }

    public static UpdateReviewReplyRequest FromBody(
        UpdateReviewReplyRequestBody requestBody,
        Guid employeeId,
        Guid reviewId,
        Guid productId,
        Guid replyId)
    {
        return new()
        {
            EmployeeId = employeeId,
            ReviewId = reviewId,
            Reply = requestBody.Reply,
            ProductId = productId,
            ReplyId = replyId
        };
    }
}
