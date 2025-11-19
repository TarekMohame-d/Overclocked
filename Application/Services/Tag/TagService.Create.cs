using System.Net;
using Application.Common.Results;
using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.Mapping;

namespace Application.Services.Tag;

public sealed partial class TagService
{
    public async Task<Result> CreateTagAsync(CreateTagRequest request, CancellationToken cancellationToken)
    {
        Domain.Entities.Tag tag = request.ToEntity();

        await tagRepository.AddAsync(tag, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
