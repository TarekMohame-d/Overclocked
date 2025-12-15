using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Review.Mapping;
using Overclocked.Contracts.Review;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using ReviewEntity = Overclocked.Domain.ReviewAggregate.Review;

namespace Overclocked.Application.Review.Queries.GetPagedReviews;

public class GetPagedReviewsQueryHandler(IReviewRepository reviewRepository)
    : IQueryHandler<GetPagedReviewsQuery, PagedResult<ReviewResponse>>
{
    public async Task<Result<PagedResult<ReviewResponse>>> Handle(
        GetPagedReviewsQuery query,
        CancellationToken cancellationToken)
    {
        var productId = ProductId.Create(query.ProductId);
        var totalCount = await reviewRepository.CountAsync(productId, cancellationToken);

        if(totalCount == 0)
        {
            return Result.Success(PagedResult<ReviewResponse>.Empty(query.Page, query.PageSize));
        }

        List<ReviewEntity> reviews = await reviewRepository.GetPagedAsync(
            query.Page,
            query.PageSize,
            query.ReviewSortField,
            query.SortDirection,
            cancellationToken);

        return Result.Success(PagedResult<ReviewResponse>.Create(
                reviews.ToDto(),
                query.Page,
                query.PageSize,
                totalCount));
    }
}
