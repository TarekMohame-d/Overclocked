using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface ICategoryReadRepository : IRepository
{
    Task<Category?> GetByIdAsync(CategoryId id, CancellationToken ct = default);

    Task<List<Category>> GetAllAsync(CancellationToken ct = default);
}
