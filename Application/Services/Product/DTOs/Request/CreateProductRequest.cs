namespace Application.Services.Product.DTOs.Request;

public record CreateProductRequest
{
    public required Guid BrandId { get; init; }
    public required Guid CategoryId { get; init; }
    public required string Name { get; init; }
    public required string Thumbnail { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required int Stock { get; init; }
    public required decimal Discount { get; init; }
    public required IEnumerable<Guid> Tags { get; init; }
    public IEnumerable<string>? Images { get; init; }
    public required IEnumerable<Specs> Specification { get; init; }

    public record Specs
    {
        public required string Name { get; init; }
        public required string Value { get; init; }
    }

}
