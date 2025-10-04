namespace Application.Features.Product.Queries.GetPagedProducts;

public record ProductListDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Thumbnail { get; init; }
    public decimal Price { get; init; }
    public decimal Discount { get; init; }
    public double Rating { get; init; }
}
