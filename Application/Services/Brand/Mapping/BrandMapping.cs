using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.DTOs.Response;
using Application.Services.Tag.DTOs.Request;
using BrandEntity = Domain.Entities.Brand;

namespace Application.Features.Brand.Mapping;

public static class BrandMapping
{
    public static BrandEntity ToEntity(this CreateBrandRequest request)
    {
        return new BrandEntity
        {
            Name = request.Name,
            Image = request.ImageUrl
        };
    }

    public static void UpdateFrom(this BrandEntity entity, UpdateBrandRequest request)
    {
        entity.Name = request.Name;
        entity.Image = request.ImageUrl;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static BrandResponse ToDto(this BrandEntity entity)
    {
        return new BrandResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            ImageUrl = entity.Image
        };
    }

    public static IEnumerable<BrandListResponse> ToDto(this IEnumerable<BrandEntity> entities)
    {
        return entities.Select(x => new BrandListResponse
        {
            Id = x.Id,
            Name = x.Name,
            ImageUrl = x.Image
        });
    }
}
