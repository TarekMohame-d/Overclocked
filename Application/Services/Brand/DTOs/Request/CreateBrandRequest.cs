namespace Application.Services.Brand.DTOs.Request;

public record CreateBrandRequest
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
