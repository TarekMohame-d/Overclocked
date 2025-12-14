using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Contracts.Brand;
using Overclocked.Domain.BrandAggregate.ValueObjects;

namespace Overclocked.Application.Brand.Queries.GetBrandById;

public record GetBrandByIdQuery : IQuery<BrandResponse>, ICachedQuery
{
    public required BrandId Id { get; init; }
    public string CacheKey => CacheKeys.Brand(Id.Value.ToString());
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
