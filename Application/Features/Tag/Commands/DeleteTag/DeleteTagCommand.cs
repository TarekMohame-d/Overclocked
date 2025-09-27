using Application.Abstraction.Messaging;
using Application.Common.Results;

namespace Application.Features.Tag.Commands.DeleteTag;

public record DeleteTagCommand : ICommand<Result>
{
    public Guid Id { get; init; }
}
