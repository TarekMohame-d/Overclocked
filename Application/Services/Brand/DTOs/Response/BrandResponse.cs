namespace Application.Services.Brand.DTOs.Response;

public record BrandResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
