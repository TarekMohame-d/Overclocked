namespace Overclocked.Contracts.Brand;

public record UpdateBrandRequest
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
