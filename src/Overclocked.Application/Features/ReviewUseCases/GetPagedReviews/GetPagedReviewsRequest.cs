using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Common.Enums;
using Overclocked.Application.Features.ReviewUseCases.DTOs.Responses;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.ReviewUseCases.GetPagedReviews;

public record GetPagedReviewsRequest : IRequest<PagedResult<ReviewResponse>>, ICachedRequest
{
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required string SortBy { get; init; }
    public required string Direction { get; init; }
    public required Guid ProductId { get; init; } = Guid.Empty;

    public ReviewSortField ReviewSortField =>
        Enum.TryParse(SortBy, true, out ReviewSortField parsedSortBy) ? parsedSortBy : ReviewSortField.Id;
    public SortDirection SortDirection =>
        Enum.TryParse(Direction, true, out SortDirection parsedDirection) ? parsedDirection : SortDirection.Asc;

    public string CacheKey =>
        CacheKeys.ReviewPaged(Page, PageSize, ReviewSortField.ToString().ToLower(), SortDirection.ToString().ToLower());
    public string CacheSetKey => CacheKeys.ReviewSet(ProductId.ToString());
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
