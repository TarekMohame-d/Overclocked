using System.Net;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;

namespace Application.Services.Tag;

public sealed partial class TagService
{
    public async Task<Result> DeleteTagAsync(Guid tagId, CancellationToken cancellationToken)
    {
        Domain.Entities.Tag? tag = await tagRepository.GetByIdAsync([tagId], cancellationToken);

        if(tag is null)
            return Result.Failure(Errors.TagNotFound, HttpStatusCode.NotFound);

        tagRepository.Delete(tag);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
