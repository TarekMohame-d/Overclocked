using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Features.Tag.Mapping;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.DTOs.Response;

namespace Application.Services.Tag;

public sealed partial class TagService
{
    public async Task<Result<PagedResult<TagListResponse>>> GetPagedTagsAsync(GetPagedTagsQuery query, CancellationToken cancellationToken)
    {
        var tagsQuery = _tagRepository.GetTagsQuery(query.SortBy, query.Direction);

        var tagsDtoQuery = tagsQuery.ToDto();

        var pagedResult = await PagedResult<TagListResponse>.CreateAsync(
            tagsDtoQuery,
            query.Page,
            query.PageSize);

        return Result<PagedResult<TagListResponse>>.Success(pagedResult);
    }
}
