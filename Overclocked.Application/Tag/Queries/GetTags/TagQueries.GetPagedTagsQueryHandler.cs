using Overclocked.Application.Tag.Mapping;
using Overclocked.Application.Tag.Queries.GetTags;
using Overclocked.Contracts.Tag;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Tag.Queries;

public sealed partial class TagQueries
{
    public async Task<Result<PagedResult<TagListResponse>>> GetPagedTagsQueryHandler(
        GetPagedTagsQuery query,
        CancellationToken cancellationToken)
    {
        var totalCount = await tagRepository.CountAsync(query.SearchTerm, cancellationToken);

        if(totalCount == 0)
        {
            return Result<PagedResult<TagListResponse>>
                .Success(PagedResult<TagListResponse>.Empty(query.Page, query.PageSize));
        }

        List<Domain.TagAggregate.Tag> tags = await tagRepository.GetTagsAsync(
            query.Page,
            query.PageSize,
            query.SearchTerm,
            query.TagSortField,
            query.SortDirection,
            cancellationToken);

        return Result<PagedResult<TagListResponse>>.Success(
            PagedResult<TagListResponse>.Create(
                tags.ToDto(),
                query.Page,
                query.PageSize,
                totalCount));
    }
}
