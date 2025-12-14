using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class CategoryRepository(ApplicationDbContext context)
    : GenericRepository<Category, CategoryId>(context), ICategoryRepository
{
    public Task<Category?> FindAsync(CategoryId id, CancellationToken cancellationToken = default)
    {
        return _dbSet.FindAsync([id], cancellationToken: cancellationToken).AsTask();
    }

    public Task<Category?> GetByIdAsync(
        CategoryId id,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public Task<List<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }
}
