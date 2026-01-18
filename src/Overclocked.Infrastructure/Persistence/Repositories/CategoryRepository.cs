using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class CategoryRepository(ApplicationDbContext dbContext) : ICategoryRepository
{
    private readonly DbSet<Category> _dbSet = dbContext.Categories;

    public Task<Category?> GetByIdAsync(CategoryId id, CancellationToken ct = default) => _dbSet.FindAsync([id], ct).AsTask();

    public Task<bool> ExistsAsync(CategoryId id, CancellationToken ct = default) => _dbSet.AnyAsync(x => x.Id == id, ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
    {
        var normalizedInput = name.Trim().ToUpperInvariant();
        return _dbSet.AnyAsync(x => x.NormalizedName == normalizedInput, ct);
    }

    public void Add(Category category) => _dbSet.Add(category);

    public void Remove(Category category) => _dbSet.Remove(category);
}
