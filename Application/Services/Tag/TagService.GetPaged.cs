using Application.Common.Results;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.DTOs.Response;
using Application.Services.Tag.Mapping;

namespace Application.Services.Tag;

public sealed partial class TagService
{
    public async Task<Result<PagedResult<TagListResponse>>> GetPagedTagsAsync(
        GetPagedTagsRequest request,
        CancellationToken cancellationToken
    )
    {
        IQueryable<Domain.Entities.Tag> tagsQuery = tagRepository.GetTagsQuery(request.SortBy, request.Direction);

        IQueryable<TagListResponse> tagsDtoQuery = tagsQuery.ToDto();

        PagedResult<TagListResponse> pagedResult = await PagedResult<TagListResponse>.CreateAsync(
            tagsDtoQuery,
            request.Page,
            request.PageSize
        );

        return Result<PagedResult<TagListResponse>>.Success(pagedResult);
    }
}
