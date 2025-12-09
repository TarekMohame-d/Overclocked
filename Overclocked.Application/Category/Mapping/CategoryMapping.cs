using Overclocked.Contracts.Category;
using CategoryEntity = Overclocked.Domain.CategoryAggregate.Category;

namespace Overclocked.Application.Category.Mapping;

public static class CategoryMapping
{
    public static CategoryResponse ToDto(this CategoryEntity entity)
    {
        return new(entity.Id, entity.Name, entity.ImageUrl);
    }

    public static IEnumerable<CategoryListResponse> ToDto(this IEnumerable<CategoryEntity> entities)
    {
        return entities.Select(x => new CategoryListResponse(x.Id, x.Name, x.ImageUrl));
    }
}
