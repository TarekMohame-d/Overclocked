using Overclocked.Application.Abstraction.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Common.Enums;

namespace Overclocked.Application.Tag.Queries.GetTags;

public record GetPagedTagsQuery(
    int Page,
    int PageSize,
    string SearchTerm,
    string SortBy,
    string Direction) : ICachedQuery
{
    public TagSortField TagSortField => Enum.TryParse(SortBy, true, out TagSortField parsedSortBy)
            ? parsedSortBy
            : TagSortField.Id;
    public SortDirection SortDirection => Enum.TryParse(Direction, true, out SortDirection parsedDirection)
            ? parsedDirection
            : SortDirection.Asc;
    public string CacheKey => CacheKeys.TagPaged(Page, PageSize, SearchTerm, SortBy, Direction);
    public string CacheSetKey => CacheKeys.TagSet;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}
