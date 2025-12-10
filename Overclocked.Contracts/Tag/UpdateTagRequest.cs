namespace Overclocked.Contracts.Tag;

public record UpdateTagRequest
{
    public required string Name { get; init; }
}
