using Application.Abstraction.Messaging;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Features.Tag.Queries.GetAllTags;

namespace Application.Features.Tag.Queries.GetPagedTags;

public record GetPagedTagsQuery : GetPagedTagsRequest, ICachedRequest<Result<PagedResult<TagListDto>>>, IValidationalRequest
{
    public string CacheKey => CacheKeys.TagPaged(Page, PageSize, SortBy);
    public string CacheSetKey => CacheKeys.TagSet;
    public bool BypassCache => false;
}

public record GetPagedTagsRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortBy { get; set; } = "id_asc";
}
