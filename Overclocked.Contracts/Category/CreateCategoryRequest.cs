namespace Overclocked.Contracts.Category;

public record CreateCategoryRequest
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
