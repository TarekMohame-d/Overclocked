namespace Overclocked.Application.Tag.Commands.CreateTag;

public record CreateTagCommand
{
    public required string Name { get; init; }
}
