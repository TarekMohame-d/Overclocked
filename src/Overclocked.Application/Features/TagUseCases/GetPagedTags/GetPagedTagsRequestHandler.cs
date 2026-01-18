using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Features.TagUseCases.DTOs.Responses;
using Overclocked.Application.Features.TagUseCases.Mapping;
using Overclocked.Domain.TagAggregate;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.TagUseCases.GetPagedTags;

public class GetPagedTagsRequestHandler(ITagReadRepository tagRepository)
    : IRequestHandler<GetPagedTagsRequest, PagedResult<TagPagedResponse>>
{
    public async Task<Result<PagedResult<TagPagedResponse>>> Handle(GetPagedTagsRequest request, CancellationToken ct)
    {
        var totalCount = await tagRepository.CountAsync(request.SearchTerm, ct);

        if (totalCount == 0)
            return Result.Success(PagedResult<TagPagedResponse>.Empty(request.Page, request.PageSize));

        List<Tag> tags = await tagRepository.GetPagedAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.TagSortField,
            request.SortDirection,
            ct
        );

        return Result.Success(PagedResult<TagPagedResponse>.Create(tags.ToDto(), request.Page, request.PageSize, totalCount));
    }
}
