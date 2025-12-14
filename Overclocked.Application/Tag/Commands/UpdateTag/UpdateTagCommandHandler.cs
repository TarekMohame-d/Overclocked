using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.TagAggregate.ValueObjects;
using TagEntity = Overclocked.Domain.TagAggregate.Tag;

namespace Overclocked.Application.Tag.Commands.UpdateTag;

public class UpdateTagCommandHandler(
    ITagRepository tagRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateTagCommand>
{
    public async Task<Result> Handle(UpdateTagCommand command, CancellationToken cancellationToken)
    {
        TagEntity? tag = await tagRepository.FindAsync(TagId.Create(command.Id), cancellationToken);

        if(tag is null)
        {
            return Result.Failure(TagErrors.TagNotFound(command.Id));
        }

        if(tag.Name != command.Name)
        {
            var exist = await tagRepository
                .AnyAsync(x => x.NormalizedName == command.Name.ToUpper(), cancellationToken);

            if(exist)
            {
                return Result.Failure(TagErrors.TagNameAlreadyExists);
            }
        }

        tag.Update(command.Name);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
