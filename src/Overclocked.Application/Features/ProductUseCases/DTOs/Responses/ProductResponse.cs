using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;
using Overclocked.Application.Features.CategoryUseCases.DTOs.Responses;
using Overclocked.Application.Features.TagUseCases.DTOs.Responses;

namespace Overclocked.Application.Features.ProductUseCases.DTOs.Responses;

public record ProductResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Thumbnail { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required decimal Discount { get; init; }
    public decimal FinalPrice => Math.Round(Price * (1 - Discount), 2, MidpointRounding.ToEven);
    public required double Rating { get; init; }
    public required int ReviewCount { get; init; }
    public required CategoryResponse Category { get; init; }
    public required BrandResponse Brand { get; init; }
    public required IEnumerable<TagResponse> Tags { get; init; }
    public required IEnumerable<ProductSpecificationDto> Specifications { get; init; }
    public IEnumerable<string>? Images { get; init; }
    public required IDictionary<int, int> RatingsBreakdown { get; init; } = new Dictionary<int, int>();
}
