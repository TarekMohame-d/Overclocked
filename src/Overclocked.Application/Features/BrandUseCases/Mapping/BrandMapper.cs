using Overclocked.Application.Features.BrandUseCases.DTOs.Responses;
using Overclocked.Domain.BrandAggregate;

namespace Overclocked.Application.Features.BrandUseCases.Mapping;

public static class BrandMapper
{
    public static BrandResponse ToDto(this Brand entity) =>
        new()
        {
            Id = entity.Id.Value,
            Name = entity.Name,
            ImageUrl = entity.Image.Value,
        };

    public static IEnumerable<BrandListResponse> ToDto(this IEnumerable<Brand> entities) =>
        entities.Select(x => new BrandListResponse
        {
            Id = x.Id.Value,
            Name = x.Name,
            ImageUrl = x.Image.Value,
        });
}
