namespace Application.Services.ReviewReply.DTOs.Request;

public record CreateReviewReplyRequestBody
{
    public required string Reply { get; init; }
}

public record CreateReviewReplyRequest : CreateReviewReplyRequestBody
{
    public required Guid ReviewId { get; init; }
    public required Guid EmployeeId { get; init; }
    public required Guid ProductId { get; init; }

    public static CreateReviewReplyRequest FromBody(
        CreateReviewReplyRequestBody requestBody,
        Guid employeeId,
        Guid reviewId,
        Guid productId)
    {
        return new()
        {
            EmployeeId = employeeId,
            ReviewId = reviewId,
            Reply = requestBody.Reply,
            ProductId = productId
        };
    }
}
