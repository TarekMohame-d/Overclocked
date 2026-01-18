using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.BrandUseCases.DeleteBrand;

public record DeleteBrandRequest : IRequest, ICacheInvalidatorRequest
{
    public required Guid Id { get; init; }

    public string[] CacheKeys => [Common.Constants.CacheKeys.Brand(Id.ToString()), Common.Constants.CacheKeys.AllBrands];

    public string? CacheSetKey => null;
}
