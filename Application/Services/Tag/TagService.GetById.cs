using System.Net;
using Application.Common.Results;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.DTOs.Response;
using Application.Services.Tag.Mapping;

namespace Application.Services.Tag;

public sealed partial class TagService
{
    public async Task<Result<TagResponse>> GetTagByIdAsync(GetTagByIdRequest request,
        CancellationToken cancellationToken)
    {
        Domain.Entities.Tag? tag = await tagRepository.GetByIdAsync([request.Id], cancellationToken);

        return tag?.ToDto() ?? Result<TagResponse>.Failure(
            Errors.TagNotFound,
            HttpStatusCode.NotFound);
    }
}
