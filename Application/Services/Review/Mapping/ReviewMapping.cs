using Application.Services.Review.DTOs.Request;
using Application.Services.Review.DTOs.Response;
using Application.Services.ReviewReply.DTOs.Request;
using ReviewEntity = Domain.Entities.Review;

namespace Application.Services.Review.Mapping;

public static class ReviewMapping
{
    public static ReviewEntity ToEntity(this CreateReviewRequest request) =>
        new()
        {
            UserId = request.UserId,
            ProductId = request.ProductId,
            Rating = request.Rating,
            Comment = request.Comment
        };

    public static void UpdateFrom(this ReviewEntity entity, UpdateReviewRequest request)
    {
        entity.Rating = request.Rating;
        entity.Comment = request.Comment;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static ReviewCreatedResponse ToDto(this ReviewEntity entity, double averageRating, int reviewCount) =>
        new()
        {
            Rating = entity.Rating,
            Comment = entity.Comment,
            CreatedAt = entity.UpdatedAt,
            AverageRating = averageRating,
            ReviewCount = reviewCount
        };

    public static ReviewUpdatedResponse ToDto(this ReviewEntity entity, int reviewCount, double averageRating) =>
        new()
        {
            Rating = entity.Rating,
            Comment = entity.Comment,
            UpdatedAt = entity.UpdatedAt,
            AverageRating = averageRating,
            ReviewCount = reviewCount
        };

    public static IQueryable<ReviewResponse> ToDto(this IQueryable<ReviewEntity> entities) =>
        entities.Select(x =>
            new ReviewResponse
            {
                Id = x.Id,
                Rating = x.Rating,
                Comment = x.Comment,
                CreatedAt = x.UpdatedAt,
                UserId = x.UserId,
                UserEmail = x.User!.Email,
                UserName = $"{x.User.FirstName} {x.User.LastName}",
                Reply = x.ReviewReply != null
                        ? new ReviewReplyResponse
                        {
                            Id = x.ReviewReply.Id,
                            Reply = x.ReviewReply.Reply ?? "",
                            CreatedAt = x.ReviewReply.UpdatedAt
                        }
                        : null
            });
}
