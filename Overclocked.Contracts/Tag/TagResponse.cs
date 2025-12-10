namespace Overclocked.Contracts.Tag;

public record TagResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
