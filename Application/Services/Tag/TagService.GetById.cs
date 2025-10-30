using System.Net;
using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Features.Tag.Mapping;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.DTOs.Response;

namespace Application.Services.Tag;

public sealed partial class TagService
{
    public async Task<Result<TagResponse>> GetTagByIdAsync(GetTagByIdRequest request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync([request.Id], cancellationToken);

        if (tag is null)
            return Result<TagResponse>.Failure(
                Errors.TagNotFound,
                HttpStatusCode.NotFound);

        return tag.ToDto();
    }
}
