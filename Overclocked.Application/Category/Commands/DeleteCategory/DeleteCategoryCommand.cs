using Overclocked.Domain.CategoryAggregate.ValueObjects;

namespace Overclocked.Application.Category.Commands.DeleteCategory;

public record DeleteCategoryCommand(CategoryId Id);
