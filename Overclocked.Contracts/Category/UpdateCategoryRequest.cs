namespace Overclocked.Contracts.Category;

public record UpdateCategoryRequest
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
