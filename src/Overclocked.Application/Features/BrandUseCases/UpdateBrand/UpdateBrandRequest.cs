using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Features.BrandUseCases.UpdateBrand;

public record UpdateBrandRequest : IRequest, ICacheInvalidatorRequest
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string ImageUrl { get; init; }

    public string[] CacheKeys => [Common.Constants.CacheKeys.Brand(Id.ToString()), Common.Constants.CacheKeys.AllCategories];
    public string? CacheSetKey => null;
}
