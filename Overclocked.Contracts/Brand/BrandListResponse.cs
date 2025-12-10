namespace Overclocked.Contracts.Brand;

public record BrandListResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
