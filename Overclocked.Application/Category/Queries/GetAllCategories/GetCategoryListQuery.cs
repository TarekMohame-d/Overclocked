using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Common.Constants;
using Overclocked.Contracts.Category;

namespace Overclocked.Application.Category.Queries.GetAllCategories;

public record GetAllCategoriesQuery : IQuery<IEnumerable<CategoryListResponse>>, ICachedQuery
{
    public string CacheKey => CacheKeys.AllCategories;
    public string? CacheSetKey => null;
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
