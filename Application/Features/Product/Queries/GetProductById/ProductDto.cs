using Application.Features.Brand.Queries.GetBrandById;
using Application.Features.Category.Queries.GetCategoryById;
using Application.Features.Product.Commands.Common.DTOs;
using Application.Features.Tag.Queries.GetTagById;

namespace Application.Features.Product.Queries.GetProductById;

public record ProductDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Thumbnail { get; init; }
    public required string Description { get; init; }
    public decimal Price { get; init; }
    public decimal Discount { get; init; }
    public double Rating { get; init; }
    public required CategoryDto Category { get; init; }
    public required BrandDto Brand { get; init; }
    public required IEnumerable<TagDto> Tags { get; init; }
    public IEnumerable<ProductReviewDto> Reviews { get; init; } = [];
    public IEnumerable<SpecificationDto> Specifications { get; init; } = [];
    public IEnumerable<string> Images { get; init; } = [];
}
