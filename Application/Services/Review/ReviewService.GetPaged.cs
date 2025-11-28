using Application.Common.Results;
using Application.Services.Review.DTOs.Request;
using Application.Services.Review.DTOs.Response;
using Application.Services.Review.Mapping;

namespace Application.Services.Review;

public sealed partial class ReviewService
{
    public async Task<Result<PagedResult<ReviewResponse>>> GetPagedReviewsAsync(
    GetPagedReviewsRequest request,
    CancellationToken cancellationToken)
    {
        IQueryable<Domain.Entities.Review> reviewsQuery = reviewRepository
            .GetReviewsQuery(request.ProductId, request.SortBy, request.Direction);

        IQueryable<ReviewResponse> reviewResponsesQuery = reviewsQuery.ToDto();

        PagedResult<ReviewResponse> pagedResult = await PagedResult<ReviewResponse>
            .CreateAsync(reviewResponsesQuery, request.Page, request.PageSize);

        return Result<PagedResult<ReviewResponse>>.Success(pagedResult);
    }
}
