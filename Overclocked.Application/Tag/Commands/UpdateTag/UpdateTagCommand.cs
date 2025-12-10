namespace Overclocked.Application.Tag.Commands.UpdateTag;

public record UpdateTagCommand
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
