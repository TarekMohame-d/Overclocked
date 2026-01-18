using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.TagAggregate;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.TagUseCases.CreateTag;

public class CreateTagRequestHandler(ITagRepository tagRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateTagRequest, Guid>
{
    public async Task<Result<Guid>> Handle(CreateTagRequest request, CancellationToken ct)
    {
        if (await tagRepository.NameExistsAsync(request.Name, ct))
            return Result.Failure<Guid>(TagErrors.TagNameAlreadyExists);

        Result<Tag> tagResult = Tag.Create(request.Name);

        if (tagResult.IsFailure)
            return Result.Failure<Guid>(tagResult.Error);

        Tag tag = tagResult.Value;
        tagRepository.Add(tag);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(tag.Id.Value);
    }
}
