namespace Application.Services.Category.DTOs.Request;

public record UpdateCategoryRequestBody
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}

public record UpdateCategoryRequest : UpdateCategoryRequestBody
{
    public required Guid Id { get; init; }
}
