namespace Application.Features.Brand.Queries.GetBrandById;

public record BrandDto
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
