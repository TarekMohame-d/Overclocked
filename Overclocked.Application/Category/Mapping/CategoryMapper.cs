using Overclocked.Contracts.Category;
using CategoryEntity = Overclocked.Domain.CategoryAggregate.Category;

namespace Overclocked.Application.Category.Mapping;

public static class CategoryMapper
{
    public static CategoryResponse ToDto(this CategoryEntity entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            ImageUrl = entity.ImageUrl
        };
    }

    public static IEnumerable<CategoryListResponse> ToDto(this IEnumerable<CategoryEntity> entities)
    {
        return entities.Select(x => new CategoryListResponse
        {
            Id = x.Id,
            Name = x.Name,
            ImageUrl = x.ImageUrl
        });
    }
}
