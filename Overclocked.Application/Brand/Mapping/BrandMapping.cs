using Overclocked.Contracts.Brand;
using BrandEntity = Overclocked.Domain.BrandAggregate.Brand;

namespace Overclocked.Application.Brand.Mapping;

public static class BrandMapping
{
    public static BrandResponse ToDto(this BrandEntity entity)
    {
        return new(entity.Id, entity.Name, entity.ImageUrl);
    }

    public static IEnumerable<BrandListResponse> ToDto(this IEnumerable<BrandEntity> entities)
    {
        return entities.Select(x => new BrandListResponse(x.Id, x.Name, x.ImageUrl));
    }
}
