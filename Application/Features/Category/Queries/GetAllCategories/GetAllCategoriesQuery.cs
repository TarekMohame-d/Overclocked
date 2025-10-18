using Application.Abstraction.Messaging;
using Application.Common.Constants;
using Application.Common.Results;
namespace Application.Features.Category.Queries.GetAllCategories;

public class GetAllCategoriesQuery() : ICachedRequest<Result<IEnumerable<CategoryListDto>>>
{
    public string CacheKey => CacheKeys.AllCategories;
    public string? CacheSetKey => null;
    public bool BypassCache => false;
}
