using Application.Common.Results;
using Application.Services.Review.DTOs.Response;

namespace Application.Services.Review;

public sealed partial class ReviewService
{
    public async Task<Result<RatingBreakdownResponse>> GetReviewRatingBreakdownAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        IDictionary<int, int> result = await reviewRepository
            .GetProductRatingsBreakdownAsync(productId, cancellationToken);

        var ratingBreakdownResponse = new RatingBreakdownResponse
        {
            Ratings = result
        };

        return Result<RatingBreakdownResponse>.Success(ratingBreakdownResponse);
    }
}
