using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.TagAggregate.ValueObjects;
using TagEntity = Overclocked.Domain.TagAggregate.Tag;

namespace Overclocked.Application.Tag.Commands.DeleteTag;

public class DeleteTagCommandHandler(
    ITagRepository tagRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteTagCommand>
{
    public async Task<Result> Handle(DeleteTagCommand command, CancellationToken cancellationToken)
    {
        TagEntity? tag = await tagRepository.FindAsync(TagId.Create(command.Id), cancellationToken);

        if(tag is null)
        {
            return Result.Failure(TagErrors.TagNotFound(command.Id));
        }

        tagRepository.Delete(tag);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
