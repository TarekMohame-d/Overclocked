using System.Net;
using Overclocked.Application.Brand.Mapping;
using Overclocked.Application.Category.Mapping;
using Overclocked.Application.Category.Queries.GetCategory;
using Overclocked.Contracts.Category;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Category.Queries;

public sealed partial class CategoryQueries
{
    public async Task<Result<CategoryResponse>> GetCategoryQueryHandler(
        GetCategoryQuery query,
        CancellationToken cancellationToken)
    {
        Domain.CategoryAggregate.Category? category = await categoryRepository
            .GetCategoryByIdAsync(query.Id, cancellationToken);

        return category is null
            ? Result<CategoryResponse>.Failure(CategoryErrors.CategoryNotFound(query.Id), HttpStatusCode.NotFound)
            : Result<CategoryResponse>.Success(category.ToDto());
    }
}
