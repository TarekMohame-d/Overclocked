using System.Net;
using Overclocked.Application.Tag.Commands.CreateTag;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.TagAggregate.ValueObjects;
using TagEntity = Overclocked.Domain.TagAggregate.Tag;

namespace Overclocked.Application.Tag.Commands;

public sealed partial class TagCommands
{
    public async Task<Result> CreateTagCommandHandler(CreateTagCommand command, CancellationToken cancellationToken)
    {
        var tag = TagEntity.Create(TagId.Create(), command.Name);

        await tagRepository.AddAsync(tag, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
