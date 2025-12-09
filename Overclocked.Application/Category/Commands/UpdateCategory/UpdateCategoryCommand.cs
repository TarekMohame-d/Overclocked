using Overclocked.Domain.CategoryAggregate.ValueObjects;

namespace Overclocked.Application.Category.Commands.UpdateCategory;

public record UpdateCategoryCommand(CategoryId Id, string Name, string ImageUrl);
