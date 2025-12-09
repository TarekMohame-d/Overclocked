using System.Net;
using Overclocked.Application.Tag.Commands.DeleteTag;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.TagAggregate.ValueObjects;
using TagEntity = Overclocked.Domain.TagAggregate.Tag;

namespace Overclocked.Application.Tag.Commands;

public sealed partial class TagCommands
{
    public async Task<Result> DeleteTagCommandHandler(DeleteTagCommand command, CancellationToken cancellationToken)
    {
        TagEntity? tag = await tagRepository.GetByIdAsync(TagId.Create(command.Id), cancellationToken);

        if(tag is null)
        {
            return Result.Failure(TagErrors.TagNotFound(command.Id), HttpStatusCode.NotFound);
        }

        tagRepository.Delete(tag);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
