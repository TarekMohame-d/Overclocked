using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Contracts.Category;
using Overclocked.Domain.CategoryAggregate.ValueObjects;

namespace Overclocked.Application.Category.Queries.GetCategoryById;

public record GetCategoryByIdQuery : IQuery<CategoryResponse>, ICachedQuery
{
    public required CategoryId Id { get; init; }
    public string CacheKey => CacheKeys.Category(Id.Value.ToString());
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
