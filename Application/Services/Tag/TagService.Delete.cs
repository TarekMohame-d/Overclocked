using System.Net;
using Application.Common.Results;
using Application.Services.Tag.DTOs.Request;

namespace Application.Services.Tag;

public sealed partial class TagService
{
    public async Task<Result> DeleteTagAsync(DeleteTagRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.Tag? tag = await tagRepository.GetByIdAsync([request.Id], cancellationToken);

        if (tag is null)
            return Result.Failure(Errors.TagNotFound, HttpStatusCode.NotFound);

        tagRepository.Delete(tag);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
