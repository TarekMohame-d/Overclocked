using Overclocked.Contracts.Brand;
using BrandEntity = Overclocked.Domain.BrandAggregate.Brand;

namespace Overclocked.Application.Brand.Mapping;

public static class BrandMapper
{
    public static BrandResponse ToDto(this BrandEntity entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            ImageUrl = entity.ImageUrl
        };
    }

    public static IEnumerable<BrandListResponse> ToDto(this IEnumerable<BrandEntity> entities)
    {
        return entities.Select(x => new BrandListResponse
        {
            Id = x.Id,
            Name = x.Name,
            ImageUrl = x.ImageUrl
        });
    }
}
