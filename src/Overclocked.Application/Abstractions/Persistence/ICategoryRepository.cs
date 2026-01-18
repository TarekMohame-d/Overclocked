using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;

namespace Overclocked.Application.Abstractions.Persistence;

public interface ICategoryRepository : IRepository
{
    Task<Category?> GetByIdAsync(CategoryId id, CancellationToken ct = default);

    Task<bool> ExistsAsync(CategoryId id, CancellationToken ct = default);

    Task<bool> NameExistsAsync(string name, CancellationToken ct = default);

    void Add(Category category);

    void Remove(Category category);
}
