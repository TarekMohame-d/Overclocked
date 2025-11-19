using Application.Abstraction.Messaging;
using Application.Common.Constants;
using Application.Common.Enums;

namespace Application.Services.Tag.DTOs.Request;

public record GetPagedTagsQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string SortBy { get; init; } = "";
    public string Direction { get; init; } = "";
}

public record GetPagedTagsRequest : GetPagedTagsQuery, ICachedRequest
{
    public new TagSortField SortBy { get; private init; }
    public new SortDirection Direction { get; private init; }
    public string CacheKey => CacheKeys.TagPaged(Page, PageSize, SortBy.ToString(), Direction.ToString());
    public string CacheSetKey => CacheKeys.TagSet;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);

    public static GetPagedTagsRequest FromQuery(GetPagedTagsQuery query)
    {
        TagSortField sortBy = Enum.TryParse(query.SortBy, true, out TagSortField parsedSortBy)
            ? parsedSortBy
            : TagSortField.Id;

        SortDirection direction = Enum.TryParse(query.Direction, true, out SortDirection parsedDirection)
            ? parsedDirection
            : SortDirection.Asc;

        return new GetPagedTagsRequest
        {
            Page = query.Page,
            PageSize = query.PageSize,
            SortBy = sortBy,
            Direction = direction,
        };
    }
}
