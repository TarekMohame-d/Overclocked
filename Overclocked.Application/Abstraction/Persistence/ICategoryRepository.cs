using Overclocked.Domain.CategoryAggregate.ValueObjects;

namespace Overclocked.Application.Abstraction.Persistence;

public interface ICategoryRepository : IGenericRepository<Domain.CategoryAggregate.Category, CategoryId>
{
    Task<Domain.CategoryAggregate.Category?> GetCategoryByIdAsync(
        CategoryId id,
        CancellationToken cancellationToken = default);

    Task<List<Domain.CategoryAggregate.Category>> GetCategoryListAsync(CancellationToken cancellationToken = default);
}
