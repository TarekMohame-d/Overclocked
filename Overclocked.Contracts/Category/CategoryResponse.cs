namespace Overclocked.Contracts.Category;

public record CategoryResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
