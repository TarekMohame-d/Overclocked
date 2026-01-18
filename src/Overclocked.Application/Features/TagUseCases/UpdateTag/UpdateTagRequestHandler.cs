using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.TagAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.TagUseCases.UpdateTag;

public class UpdateTagRequestHandler(ITagRepository tagRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateTagRequest>
{
    public async Task<Result> Handle(UpdateTagRequest request, CancellationToken ct)
    {
        Tag? tag = await tagRepository.GetByIdAsync(TagId.Create(request.Id), ct);

        if (tag is null)
            return Result.Failure(TagErrors.TagNotFound(request.Id));

        if (tag.Name != request.Name)
        {
            var exist = await tagRepository.NameExistsAsync(request.Name, ct);

            if (exist)
                return Result.Failure(TagErrors.TagNameAlreadyExists);
        }

        Result result = tag.Update(request.Name);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
