namespace Overclocked.Contracts.Brand;

public record BrandResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
