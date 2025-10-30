namespace Application.Services.Category.DTOs.Request;

public record CreateCategoryRequest
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
