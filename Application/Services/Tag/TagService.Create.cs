using System.Net;
using Application.Common.Results;
using Application.Features.Brand.Mapping;
using Application.Features.Tag.Mapping;
using Application.Services.Tag.DTOs.Request;

namespace Application.Services.Tag;

public sealed partial class TagService
{
    public async Task<Result> CreateTagAsync(CreateTagRequest request, CancellationToken cancellationToken)
    {
        var tag = request.ToEntity();

        await _tagRepository.AddAsync(tag, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
