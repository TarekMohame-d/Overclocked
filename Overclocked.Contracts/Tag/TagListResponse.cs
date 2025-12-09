namespace Overclocked.Contracts.Tag;

public record TagListResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
