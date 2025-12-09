using Overclocked.Application.Category.Queries.GetAllCategories;
using Overclocked.Application.Category.Queries.GetCategory;
using Overclocked.Contracts.Category;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Category.Queries;

public interface ICategoryQueries
{
    Task<Result<CategoryResponse>> GetCategoryQueryHandler(GetCategoryQuery query, CancellationToken cancellationToken);
    Task<Result<IEnumerable<CategoryListResponse>>> GetCategoryListQueryHandler(
        GetCategoryListQuery query,
        CancellationToken cancellationToken);
}
