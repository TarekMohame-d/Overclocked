using Application.Services.Brand.DTOs.Response;
using Application.Services.Category.DTOs.Response;
using Application.Services.Review.DTOs.Response;
using Application.Services.Tag.DTOs.Response;

namespace Application.Services.Product.DTOs.Response;

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
    public required IEnumerable<ReviewResponse> Reviews { get; init; } = [];
    public required IEnumerable<ProductSpecificationResponse> Specifications { get; init; }
    public IEnumerable<string>? Images { get; init; }
    public RatingBreakdownResponse RatingBreakdownResponse { get; set; } = new RatingBreakdownResponse
    {
        Ratings = new Dictionary<int, int>()
    };
}
