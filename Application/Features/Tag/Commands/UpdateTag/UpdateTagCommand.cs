using Application.Abstraction.Messaging;
using Application.Common.Results;

namespace Application.Features.Tag.Commands.UpdateTag;

public record UpdateTagWithIdCommand : UpdateTagCommand, ICommand<Result>
{
    public Guid Id { get; init; }
}

public record UpdateTagCommand
{
    public string Name { get; init; } = string.Empty;
}
