namespace Overclocked.Application.Category.Commands.UpdateCategory;

public record UpdateCategoryCommand
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }
}
