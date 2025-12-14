using Overclocked.Domain.CategoryAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface ICategoryRepository : IGenericRepository<Domain.CategoryAggregate.Category, CategoryId>
{
    Task<Domain.CategoryAggregate.Category?> GetByIdAsync(
        CategoryId id,
        CancellationToken cancellationToken = default);

    Task<List<Domain.CategoryAggregate.Category>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Domain.CategoryAggregate.Category?> FindAsync(CategoryId id, CancellationToken cancellationToken = default);
}
