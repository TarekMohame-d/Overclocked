using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.BrandUseCases.CreateBrand;

public record CreateBrandRequest : IRequest<Guid>, ICacheInvalidatorRequest
{
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }

    public string[] CacheKeys => [Common.Constants.CacheKeys.AllBrands];
    public string? CacheSetKey => null;
}
