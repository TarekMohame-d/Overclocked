using System.Net;
using Application.Common.Results;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.Mapping;

namespace Application.Services.Tag;

public sealed partial class TagService
{
    public async Task<Result> UpdateTagAsync(UpdateTagRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.Tag? tag = await tagRepository.GetByIdAsync([request.Id], cancellationToken);

        if (tag is null)
            return Result.Failure(Errors.TagNotFound, HttpStatusCode.NotFound);

        if (tag.Name != request.Name)
        {
            var exist = await tagRepository
                .AnyAsync(x => x.NormalizedName == request.Name.ToUpper(), cancellationToken);

            if (exist)
                return Result.Failure(Errors.TagNameAlreadyExists, HttpStatusCode.Conflict);
        }

        tag.UpdateFrom(request);

        tagRepository.Update(tag);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
