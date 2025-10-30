namespace Application.Services.Category.DTOs.Response;

public record CategoryListResponse
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
