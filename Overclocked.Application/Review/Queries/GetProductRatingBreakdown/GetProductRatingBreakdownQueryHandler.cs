using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Contracts.Review;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Application.Review.Queries.GetProductRatingBreakdown;

public class GetProductRatingBreakdownQueryHandler(IReviewRepository reviewRepository)
    : IQueryHandler<GetProductRatingBreakdownQuery, RatingBreakdownResponse>
{
    public async Task<Result<RatingBreakdownResponse>> Handle(
        GetProductRatingBreakdownQuery query,
        CancellationToken cancellationToken)
    {
        var productId = ProductId.Create(query.ProductId);

        IDictionary<int, int> result = await reviewRepository
            .GetProductRatingsBreakdownAsync(productId, cancellationToken);

        var ratingBreakdownResponse = new RatingBreakdownResponse
        {
            Ratings = result
        };

        return Result.Success(ratingBreakdownResponse);
    }
}
