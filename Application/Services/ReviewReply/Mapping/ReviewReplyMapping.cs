using Application.Services.ReviewReply.DTOs.Request;
using ReviewReplyEntity = Domain.Entities.ReviewReply;

namespace Application.Services.ReviewReply.Mapping;

public static class ReviewReplyMapping
{
    public static ReviewReplyEntity ToEntity(this CreateReviewReplyRequest request) =>
        new()
        {
            ReviewId = request.ReviewId,
            EmployeeId = request.EmployeeId,
            Reply = request.Reply
        };

    public static void UpdateFrom(this ReviewReplyEntity entity, UpdateReviewReplyRequest request)
    {
        entity.Reply = request.Reply;
        entity.EmployeeId = request.EmployeeId;
        entity.UpdatedAt = DateTime.UtcNow;
    }
}
