using Application.Features.Brand.Commands.CreateBrand;
using Application.Features.Brand.Commands.UpdateBrand;
using Application.Features.Brand.Queries.GetAllBrands;
using Application.Features.Brand.Queries.GetBrandById;
using BrandEntity = Domain.Entities.Brand;

namespace Application.Features.Brand.Mapping;

public static class BrandMapping
{
    public static BrandEntity ToEntity(this CreateBrandCommand command)
    {
        return new BrandEntity
        {
            Name = command.Name,
            Image = command.ImageUrl
        };
    }

    public static void UpdateFrom(this BrandEntity entity, UpdateBrandWithIdCommand command)
    {
        entity.Name = command.Name;
        entity.Image = command.ImageUrl;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static BrandDto ToDto(this BrandEntity entity)
    {
        return new BrandDto
        {
            Id = entity.Id,
            Name = entity.Name,
            ImageUrl = entity.Image
        };
    }

    public static IEnumerable<BrandListDto> ToDto(this IEnumerable<BrandEntity> entities)
    {
        return entities.Select(x => new BrandListDto
        {
            Id = x.Id,
            Name = x.Name,
            ImageUrl = x.Image
        });
    }
}
