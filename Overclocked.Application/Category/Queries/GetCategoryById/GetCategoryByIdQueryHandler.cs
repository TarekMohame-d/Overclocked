using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Brand.Mapping;
using Overclocked.Application.Category.Mapping;
using Overclocked.Contracts.Category;
using Overclocked.Domain.Common.Errors;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Category.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository)
    : IQueryHandler<GetCategoryByIdQuery, CategoryResponse>
{
    public async Task<Result<CategoryResponse>> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
    {
        Domain.CategoryAggregate.Category? category = await categoryRepository
            .GetByIdAsync(query.Id, cancellationToken);

        return category is null
            ? Result.Failure<CategoryResponse>(CategoryErrors.CategoryNotFound(query.Id))
            : Result.Success(category.ToDto());
    }
}
