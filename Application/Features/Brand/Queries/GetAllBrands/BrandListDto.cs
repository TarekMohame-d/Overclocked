namespace Application.Features.Brand.Queries.GetAllBrands;

public record BrandListDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
