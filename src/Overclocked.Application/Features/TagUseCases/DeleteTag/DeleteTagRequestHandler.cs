using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.TagAggregate.ValueObjects;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.TagUseCases.DeleteTag;

public class DeleteTagRequestHandler(ITagRepository tagRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteTagRequest>
{
    public async Task<Result> Handle(DeleteTagRequest request, CancellationToken ct)
    {
        Tag? tag = await tagRepository.GetByIdAsync(TagId.Create(request.Id), ct);

        if (tag is null)
            return Result.Failure(TagErrors.TagNotFound(request.Id));

        tagRepository.Remove(tag);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
