using Application.Abstraction.Messaging;
using Application.Common.Results;
using Application.Features.Tag.Mapping;
using Application.Features.Tag.Queries.GetAllTags;
using Domain.Repositories;

namespace Application.Features.Tag.Queries.GetPagedTags;

public class GetPagedTagsQueryHandler : IQueryHandler<GetPagedTagsQuery, Result<PagedResult<TagListDto>>>
{
    private readonly ITagRepository _tagRepository;

    public GetPagedTagsQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<Result<PagedResult<TagListDto>>> Handle(GetPagedTagsQuery query, CancellationToken cancellationToken)
    {
        var tagsQuery = _tagRepository.GetTagsQuery(query.SortBy);
        var tagsDtoQuery = tagsQuery.ToDto();
        var pagedResult = await PagedResult<TagListDto>.CreateAsync(tagsDtoQuery, query.Page, query.PageSize);
        return Result<PagedResult<TagListDto>>.Success(pagedResult);
    }
}
