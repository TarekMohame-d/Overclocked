using Overclocked.Contracts.Brand;
using Overclocked.Contracts.Category;
using Overclocked.Contracts.Review;
using Overclocked.Contracts.Tag;

namespace Overclocked.Contracts.Product;

public record ProductResponse
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Thumbnail { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required decimal Discount { get; init; }
    public required decimal FinalPrice { get; init; }
    public required double Rating { get; init; }
    public required int ReviewCount { get; init; }
    public required CategoryResponse Category { get; init; }
    public required BrandResponse Brand { get; init; }
    public required IEnumerable<TagResponse> Tags { get; init; }
    public required IEnumerable<ProductSpecificationDto> Specifications { get; init; }
    public IEnumerable<string>? Images { get; init; }
    public required IDictionary<int, int> RatingsBreakdown { get; init; } = new Dictionary<int, int>();
}
