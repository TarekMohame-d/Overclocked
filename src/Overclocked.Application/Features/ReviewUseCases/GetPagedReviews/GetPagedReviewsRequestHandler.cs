using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.ReviewUseCases.DTOs.Responses;
using Overclocked.Application.Features.ReviewUseCases.Mapping;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.ReviewAggregate;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.ReviewUseCases.GetPagedReviews;

public class GetPagedReviewsRequestHandler(IReviewReadRepository reviewRepository)
    : IRequestHandler<GetPagedReviewsRequest, PagedResult<ReviewResponse>>
{
    public async Task<Result<PagedResult<ReviewResponse>>> Handle(GetPagedReviewsRequest request, CancellationToken ct)
    {
        var productId = ProductId.Create(request.ProductId);

        var totalCount = await reviewRepository.CountAsync(productId, ct);

        if (totalCount == 0)
            return Result.Success(PagedResult<ReviewResponse>.Empty(request.Page, request.PageSize));

        List<Review> reviews = await reviewRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.ReviewSortField,
            request.SortDirection,
            ct
        );

        return Result.Success(PagedResult<ReviewResponse>.Create(reviews.ToDto(), request.Page, request.PageSize, totalCount));
    }
}
