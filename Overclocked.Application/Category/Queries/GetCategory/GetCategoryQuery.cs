using Overclocked.Application.Abstraction.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Domain.CategoryAggregate.ValueObjects;

namespace Overclocked.Application.Category.Queries.GetCategory;

public record GetCategoryQuery : ICachedQuery
{
    public required CategoryId Id { get; init; }
    public string CacheKey => CacheKeys.Category(Id.Value.ToString());
    public string? CacheSetKey => null;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}
