using Overclocked.Application.Review.Commands.CreateReview;
using Overclocked.Contracts.Review;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;
using ReviewEntity = Overclocked.Domain.ReviewAggregate.Review;

namespace Overclocked.Application.Review.Mapping;

public static class ReviewMapper
{
    public static ReviewEntity ToEntity(this CreateReviewCommand command) =>
        ReviewEntity.Create(
            userId: UserId.Create(command.UserId),
            productId: ProductId.Create(command.ProductId),
            comment: command.Comment,
            rating: command.Rating
        );

    public static IEnumerable<ReviewResponse> ToDto(this IEnumerable<ReviewEntity> entities)
    {
        return entities.Select(x => new ReviewResponse
        {
            Id = x.Id,
            Comment = x.Comment,
            Rating = x.Rating,
            CreatedAt = x.UpdatedAt,
            UserId = x.UserId.Value,
            UserName = $"{x.User.FirstName} {x.User.LastName}",
            UserEmail = x.User.Email
        });
    }
}
