namespace Overclocked.Application.Category.Commands.CreateCategory;

public record CreateCategoryCommand
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
