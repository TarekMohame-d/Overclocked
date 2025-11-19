using Application.Common.Results;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.DTOs.Response;

namespace Application.Abstraction.DomainServices;

public interface ITagService
{
    Task<Result<TagResponse>> GetTagByIdAsync(GetTagByIdRequest request, CancellationToken cancellationToken);

    Task<Result<PagedResult<TagListResponse>>> GetPagedTagsAsync(
        GetPagedTagsRequest request,
        CancellationToken cancellationToken
    );

    Task<Result> CreateTagAsync(CreateTagRequest request, CancellationToken cancellationToken);
    Task<Result> UpdateTagAsync(UpdateTagRequest request, CancellationToken cancellationToken);
    Task<Result> DeleteTagAsync(DeleteTagRequest request, CancellationToken cancellationToken);
}
