using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Common.Enums;
using Overclocked.Contracts.Review;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Review.Queries.GetPagedReviews;

public record GetPagedReviewsQuery : IQuery<PagedResult<ReviewResponse>>, ICachedQuery
{
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required string SortBy { get; init; }
    public required string Direction { get; init; }
    public required Guid ProductId { get; init; } = Guid.Empty;
    public ReviewSortField ReviewSortField => Enum.TryParse(SortBy, true, out ReviewSortField parsedSortBy)
            ? parsedSortBy
            : ReviewSortField.Id;
    public SortDirection SortDirection => Enum.TryParse(Direction, true, out SortDirection parsedDirection)
            ? parsedDirection
            : SortDirection.Asc;
    public string CacheKey => CacheKeys.ReviewPaged(Page, PageSize, SortBy, Direction);
    public string CacheSetKey => CacheKeys.ReviewSet;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);

    public static GetPagedReviewsQuery ToQuery(GetPagedReviewsRequest request, Guid productId)
    {
        return new GetPagedReviewsQuery
        {
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 10,
            SortBy = request.SortBy ?? string.Empty,
            Direction = request.Direction ?? string.Empty,
            ProductId = productId
        };
    }
}
