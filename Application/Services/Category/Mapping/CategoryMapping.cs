using Application.Services.Category.DTOs.Request;
using Application.Services.Category.DTOs.Response;
using CategoryEntity = Domain.Entities.Category;

namespace Application.Services.Category.Mapping;

public static class CategoryMapping
{
    public static CategoryEntity ToEntity(this CreateCategoryRequest request) => new CategoryEntity { Name = request.Name, Image = request.ImageUrl };

    public static void UpdateFrom(this CategoryEntity entity, UpdateCategoryRequest request)
    {
        entity.Name = request.Name;
        entity.Image = request.ImageUrl;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static CategoryResponse ToDto(this CategoryEntity entity) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            ImageUrl = entity.Image,
        };

    public static IEnumerable<CategoryListResponse> ToDto(this IEnumerable<CategoryEntity> entities) =>
        entities.Select(x => new CategoryListResponse
        {
            Id = x.Id,
            Name = x.Name,
            ImageUrl = x.Image,
        });
}
