using Overclocked.Domain.Common.Primitives;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate.Entities;
using Overclocked.Domain.ReviewAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Domain.ReviewAggregate;

public class Review : AggregateRoot<ReviewId>
{
    public UserId UserId { get; private set; }
    public ProductId ProductId { get; private set; }
    public string Comment { get; private set; }
    public int Rating { get; private set; }
    public DateTime CreatedAt { get; private init; }
    public DateTime UpdatedAt { get; private set; }
    public ReviewReply? ReviewReply { get; private set; }

    private Review()
    {
    }
    private Review(
        ReviewId id,
        UserId userId,
        ProductId productId,
        string comment,
        int rating,
        ReviewReply? reviewReply = null) : base(id)
    {
        UserId = userId;
        ProductId = productId;
        ReviewReply = reviewReply;
        Comment = comment;
        Rating = rating;

        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public static Review Create(
        ReviewId reviewId,
        UserId userId,
        ProductId productId,
        string comment,
        int rating,
        ReviewReply? reviewReply = null)
    {
        return new(reviewId, userId, productId, comment, rating, reviewReply);
    }
}
