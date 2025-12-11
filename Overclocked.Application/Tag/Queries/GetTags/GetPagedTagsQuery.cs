using Overclocked.Application.Abstraction.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Common.Enums;
using Overclocked.Contracts.Tag;

namespace Overclocked.Application.Tag.Queries.GetTags;

public record GetPagedTagsQuery : ICachedQuery
{
    public required int Page { get; init; } = 1;
    public required int PageSize { get; init; } = 10;
    public required string SearchTerm { get; init; } = string.Empty;
    public required string SortBy { get; init; } = string.Empty;
    public required string Direction { get; init; } = string.Empty;
    public TagSortField TagSortField => Enum.TryParse(SortBy, true, out TagSortField parsedSortBy)
            ? parsedSortBy
            : TagSortField.Id;
    public SortDirection SortDirection => Enum.TryParse(Direction, true, out SortDirection parsedDirection)
            ? parsedDirection
            : SortDirection.Asc;
    public string CacheKey => CacheKeys.TagPaged(Page, PageSize, SearchTerm, SortBy, Direction);
    public string CacheSetKey => CacheKeys.TagSet;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);

    public static GetPagedTagsQuery ToQuery(GetPagedTagsRequest request)
    {
        return new GetPagedTagsQuery
        {
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 10,
            SearchTerm = request.SearchTerm ?? string.Empty,
            SortBy = request.SortBy ?? string.Empty,
            Direction = request.Direction ?? string.Empty
        };
    }
}
