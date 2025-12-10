using System.Net;
using Overclocked.Application.Tag.Commands.UpdateTag;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.TagAggregate.ValueObjects;
using TagEntity = Overclocked.Domain.TagAggregate.Tag;

namespace Overclocked.Application.Tag.Commands;

public sealed partial class TagCommands
{
    public async Task<Result> UpdateTagCommandHandler(UpdateTagCommand command, CancellationToken cancellationToken)
    {
        TagEntity? tag = await tagRepository
            .SingleOrDefaultAsync(x => x.Id == TagId.Create(command.Id), asNoTracking: false, cancellationToken);

        if(tag is null)
        {
            return Result.Failure(TagErrors.TagNotFound(command.Id), HttpStatusCode.NotFound);
        }

        if(tag.Name != command.Name)
        {
            var exist = await tagRepository
                .AnyAsync(x => x.NormalizedName == command.Name.ToUpper(), cancellationToken);

            if(exist)
            {
                return Result.Failure(TagErrors.TagNameAlreadyExists, HttpStatusCode.Conflict);
            }
        }

        tag.Update(command.Name);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
