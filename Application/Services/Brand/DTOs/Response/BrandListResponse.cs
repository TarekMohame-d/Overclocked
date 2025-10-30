namespace Application.Services.Brand.DTOs.Response;

public record BrandListResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
