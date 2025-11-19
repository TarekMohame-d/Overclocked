using Application.Services.Brand.DTOs.Response;

namespace Application.Services.Product.DTOs.Response;

public record ProductListResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Thumbnail { get; init; }
    public required decimal Price { get; init; }
    public required decimal Discount { get; init; }
    public required double Rating { get; init; }
    public required BrandResponse Brand { get; init; }
}
