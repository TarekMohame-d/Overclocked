using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.ReviewUseCases.DTOs.Responses;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.ReviewUseCases.GetProductRatingBreakdown;

public class GetProductRatingBreakdownRequestHandler(IReviewReadRepository reviewRepository)
    : IRequestHandler<GetProductRatingBreakdownRequest, RatingBreakdownResponse>
{
    public async Task<Result<RatingBreakdownResponse>> Handle(GetProductRatingBreakdownRequest request, CancellationToken ct)
    {
        var productId = ProductId.Create(request.ProductId);

        IDictionary<int, int> result = await reviewRepository.GetProductRatingsBreakdownAsync(productId, ct);

        var ratingBreakdownResponse = new RatingBreakdownResponse { Ratings = result };

        return Result.Success(ratingBreakdownResponse);
    }
}
