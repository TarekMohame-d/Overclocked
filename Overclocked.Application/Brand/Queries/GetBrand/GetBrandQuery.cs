using Overclocked.Application.Abstraction.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Domain.BrandAggregate.ValueObjects;

namespace Overclocked.Application.Brand.Queries.GetBrand;

public record GetBrandQuery : ICachedQuery
{
    public required BrandId Id { get; init; }
    public string CacheKey => CacheKeys.Brand(Id.Value.ToString());
    public string? CacheSetKey => null;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}
