using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Brand.Mapping;
using Overclocked.Application.Category.Mapping;
using Overclocked.Contracts.Category;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Category.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler(ICategoryRepository categoryRepository)
    : IQueryHandler<GetAllCategoriesQuery, IEnumerable<CategoryListResponse>>
{
    public async Task<Result<IEnumerable<CategoryListResponse>>> Handle(
        GetAllCategoriesQuery query,
        CancellationToken cancellationToken)
    {
        IEnumerable<Domain.CategoryAggregate.Category> result = await categoryRepository
            .GetAllAsync(cancellationToken);

        return Result.Success(result.ToDto());
    }
}
