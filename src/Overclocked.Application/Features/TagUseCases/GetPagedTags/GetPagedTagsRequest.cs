using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Common.Enums;
using Overclocked.Application.Features.TagUseCases.DTOs.Responses;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.TagUseCases.GetPagedTags;

public record GetPagedTagsRequest : IRequest<PagedResult<TagPagedResponse>>, ICachedRequest
{
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required string SearchTerm { get; init; }
    public required string SortBy { get; init; }
    public required string Direction { get; init; }
    public TagSortField TagSortField =>
        Enum.TryParse(SortBy, true, out TagSortField parsedSortBy) ? parsedSortBy : TagSortField.Id;

    public SortDirection SortDirection =>
        Enum.TryParse(Direction, true, out SortDirection parsedDirection) ? parsedDirection : SortDirection.Asc;

    public string CacheKey =>
        CacheKeys.TagPaged(Page, PageSize, SearchTerm, TagSortField.ToString().ToLower(), SortDirection.ToString().ToLower());
    public string CacheSetKey => CacheKeys.TagSet;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
