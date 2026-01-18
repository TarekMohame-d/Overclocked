using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.ProductUseCases.DTOs.Requests;

public record CreateProductRequestDto
{
    public required Guid BrandId { get; init; }
    public required Guid CategoryId { get; init; }
    public required string Name { get; init; }
    public required string Thumbnail { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required int StockQuantity { get; init; }
    public decimal? Discount { get; init; }
    public required List<Guid> Tags { get; init; }
    public required List<ProductSpecificationDto> Specifications { get; init; }
    public List<string>? Images { get; init; }
}
