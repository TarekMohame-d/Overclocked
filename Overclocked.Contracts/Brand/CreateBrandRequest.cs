namespace Overclocked.Contracts.Brand;

public record CreateBrandRequest
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
