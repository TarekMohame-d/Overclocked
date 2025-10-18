using Application.Abstraction.Messaging;
using Application.Common.Results;

namespace Application.Features.Tag.Commands.CreateTag;

public record CreateTagCommand : ICommand<Result>
{
    public required string Name { get; init; }
}
