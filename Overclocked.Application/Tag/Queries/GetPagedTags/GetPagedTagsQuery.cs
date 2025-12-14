using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Common.Enums;
using Overclocked.Contracts.Tag;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Tag.Queries.GetPagedTags;

public record GetPagedTagsQuery : IQuery<PagedResult<TagPagedResponse>>, ICachedQuery
{
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required string SearchTerm { get; init; }
    public required string SortBy { get; init; }
    public required string Direction { get; init; }
    public TagSortField TagSortField => Enum.TryParse(SortBy, true, out TagSortField parsedSortBy)
            ? parsedSortBy
            : TagSortField.Id;
    public SortDirection SortDirection => Enum.TryParse(Direction, true, out SortDirection parsedDirection)
            ? parsedDirection
            : SortDirection.Asc;
    public string CacheKey => CacheKeys.TagPaged(Page, PageSize, SearchTerm, SortBy, Direction);
    public string CacheSetKey => CacheKeys.TagSet;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);

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
