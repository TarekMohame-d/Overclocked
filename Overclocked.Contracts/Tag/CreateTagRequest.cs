namespace Overclocked.Contracts.Tag;

public record CreateTagRequest
{
    public required string Name { get; init; }
}
