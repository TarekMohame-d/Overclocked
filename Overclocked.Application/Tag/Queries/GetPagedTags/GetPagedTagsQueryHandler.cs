using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Tag.Mapping;
using Overclocked.Contracts.Tag;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Tag.Queries.GetPagedTags;

public class GetPagedTagsQueryHandler(ITagRepository tagRepository)
    : IQueryHandler<GetPagedTagsQuery, PagedResult<TagPagedResponse>>
{
    public async Task<Result<PagedResult<TagPagedResponse>>> Handle(
        GetPagedTagsQuery query,
        CancellationToken cancellationToken)
    {
        var totalCount = await tagRepository.CountAsync(query.SearchTerm, cancellationToken);

        if(totalCount == 0)
        {
            return Result.Success(PagedResult<TagPagedResponse>.Empty(query.Page, query.PageSize));
        }

        List<Domain.TagAggregate.Tag> tags = await tagRepository.GetPagedAsync(
            query.Page,
            query.PageSize,
            query.SearchTerm,
            query.TagSortField,
            query.SortDirection,
            cancellationToken);

        return Result.Success(PagedResult<TagPagedResponse>.Create(
                tags.ToDto(),
                query.Page,
                query.PageSize,
                totalCount));
    }
}
