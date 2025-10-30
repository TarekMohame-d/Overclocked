using System.Net;
using Application.Common.Results;
using Application.Services.Tag.DTOs.Request;

namespace Application.Services.Tag;

public sealed partial class TagService
{
    public async Task<Result> DeleteTagAsync(DeleteTagRequest request, CancellationToken cancellationToken)
    {
        var tag = await _tagRepository.GetByIdAsync([request.Id], cancellationToken);

        if (tag is null)
            return Result.Failure(Errors.TagNotFound, HttpStatusCode.NotFound);

        _tagRepository.Delete(tag);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
