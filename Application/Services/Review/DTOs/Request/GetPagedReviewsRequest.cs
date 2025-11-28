using Application.Common.Enums;

namespace Application.Services.Review.DTOs.Request;

public record GetPagedReviewsQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SortBy { get; init; } = "createdAt";
    public string Direction { get; init; } = "desc";
}

public record GetPagedReviewsRequest : GetPagedReviewsQuery
{
    public required Guid ProductId { get; init; }
    public new ReviewSortField SortBy { get; private init; }
    public new SortDirection Direction { get; private init; }

    public static GetPagedReviewsRequest FromQuery(GetPagedReviewsQuery query, Guid productId)
    {
        ReviewSortField sortBy = Enum.TryParse(query.SortBy, true, out ReviewSortField parsedSortBy)
            ? parsedSortBy
            : ReviewSortField.CreatedAt;

        SortDirection direction = Enum.TryParse(query.Direction, true, out SortDirection parsedDirection)
            ? parsedDirection
            : SortDirection.Desc;

        return new GetPagedReviewsRequest
        {
            ProductId = productId,
            Page = query.Page,
            PageSize = query.PageSize,
            SortBy = sortBy,
            Direction = direction
        };
    }
}
