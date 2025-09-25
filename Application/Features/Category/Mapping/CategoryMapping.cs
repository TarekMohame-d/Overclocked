using Application.Features.Category.Commands.CreateCategory;
using Application.Features.Category.Commands.UpdateCategory;
using Application.Features.Category.Queries.GetAllCategories;
using Application.Features.Category.Queries.GetCategoryById;
using CategoryEntity = Domain.Entities.Category;

namespace Application.Features.Category.Mapping;

public static class CategoryMapping
{
    public static CategoryEntity ToEntity(this CreateCategoryCommand command)
    {
        return new CategoryEntity
        {
            Name = command.Name,
            Image = command.ImageUrl
        };
    }

    public static void UpdateFrom(this CategoryEntity entity, UpdateCategoryWithIdCommand command)
    {
        entity.Name = command.Name;
        entity.Image = command.ImageUrl;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static CategoryDto ToDto(this CategoryEntity entity)
    {
        return new CategoryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            ImageUrl = entity.Image
        };
    }

    public static IEnumerable<CategoryListDto> ToDto(this IEnumerable<CategoryEntity> entities)
    {
        return entities.Select(x => new CategoryListDto
        {
            Id = x.Id,
            Name = x.Name,
            ImageUrl = x.Image
        });
    }
}
