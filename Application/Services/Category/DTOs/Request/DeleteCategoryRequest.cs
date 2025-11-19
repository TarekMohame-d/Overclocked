namespace Application.Services.Category.DTOs.Request;

public record DeleteCategoryRequest
{
    public required Guid Id { get; init; }
}
