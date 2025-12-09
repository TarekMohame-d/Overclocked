using Overclocked.Application.Abstraction.Messaging;
using Overclocked.Application.Common.Constants;

namespace Overclocked.Application.Category.Queries.GetAllCategories;

public record GetCategoryListQuery : ICachedQuery
{
    public string CacheKey => CacheKeys.AllCategories;
    public string? CacheSetKey => null;
    public TimeSpan SlidingExpiration => TimeSpan.FromMinutes(5);
}
