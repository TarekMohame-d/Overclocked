using Overclocked.Application.Brand.Mapping;
using Overclocked.Application.Category.Mapping;
using Overclocked.Application.Category.Queries.GetAllCategories;
using Overclocked.Contracts.Category;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Category.Queries;

public sealed partial class CategoryQueries
{
    public async Task<Result<IEnumerable<CategoryListResponse>>> GetCategoryListQueryHandler(
        GetCategoryListQuery query,
        CancellationToken cancellationToken)
    {
        IEnumerable<Domain.CategoryAggregate.Category> result = await categoryRepository
            .GetCategoryListAsync(cancellationToken);

        return Result<IEnumerable<CategoryListResponse>>.Success(result.ToDto());
    }
}
