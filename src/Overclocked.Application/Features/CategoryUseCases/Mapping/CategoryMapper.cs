using Overclocked.Application.Features.CategoryUseCases.DTOs.Responses;
using Overclocked.Domain.CategoryAggregate;

namespace Overclocked.Application.Features.CategoryUseCases.Mapping;

public static class CategoryMapper
{
    public static CategoryResponse ToDto(this Category entity) =>
        new()
        {
            Id = entity.Id.Value,
            Name = entity.Name,
            ImageUrl = entity.Image.Value,
        };

    public static IEnumerable<CategoryListResponse> ToDto(this IEnumerable<Category> entities) =>
        entities.Select(x => new CategoryListResponse
        {
            Id = x.Id.Value,
            Name = x.Name,
            ImageUrl = x.Image.Value,
        });
}
