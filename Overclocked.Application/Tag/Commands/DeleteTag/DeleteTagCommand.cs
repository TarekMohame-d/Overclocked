namespace Overclocked.Application.Tag.Commands.DeleteTag;

public record DeleteTagCommand
{
    public required Guid Id { get; init; }
}
