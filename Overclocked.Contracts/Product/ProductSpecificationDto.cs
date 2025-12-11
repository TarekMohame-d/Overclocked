namespace Overclocked.Contracts.Product;

public record ProductSpecificationDto
{
    public required string Name { get; init; }
    public required string Value { get; init; }
}
