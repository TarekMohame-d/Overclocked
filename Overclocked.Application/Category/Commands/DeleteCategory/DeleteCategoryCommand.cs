namespace Overclocked.Application.Category.Commands.DeleteCategory;

public record DeleteCategoryCommand
{
    public required Guid Id { get; init; }
}
