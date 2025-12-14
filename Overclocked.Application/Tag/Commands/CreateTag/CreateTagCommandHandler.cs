using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.TagAggregate.ValueObjects;
using TagEntity = Overclocked.Domain.TagAggregate.Tag;

namespace Overclocked.Application.Tag.Commands.CreateTag;

public class CreateTagCommandHandler(
    ITagRepository tagRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateTagCommand>
{
    public async Task<Result> Handle(CreateTagCommand command, CancellationToken cancellationToken)
    {
        var tag = TagEntity.Create(TagId.Create(), command.Name);

        await tagRepository.AddAsync(tag, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
