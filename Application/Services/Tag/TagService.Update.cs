using System.Net;
using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Features.Tag.Mapping;
using Application.Services.Tag.DTOs.Request;

namespace Application.Services.Tag;

public sealed partial class TagService
{
    public async Task<Result> UpdateTagAsync(UpdateTagRequest request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync([request.Id], cancellationToken);

        if (tag is null)
            return Result.Failure(Errors.TagNotFound, HttpStatusCode.NotFound);

        if (tag.Name != request.Name)
        {
            bool exist = await _tagRepository
                .AnyAsync(x => x.NormalizedName == request.Name.ToUpper(), cancellationToken);

            if (exist)
                return Result.Failure(Errors.TagNameAlreadyExists, HttpStatusCode.Conflict);
        }

        tag.UpdateFrom(request);

        _tagRepository.Update(tag);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
