namespace Overclocked.Contracts.Tag;

public record TagPagedResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
