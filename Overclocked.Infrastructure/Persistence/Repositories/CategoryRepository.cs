using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class CategoryRepository(ApplicationDbContext context)
    : GenericRepository<Category, CategoryId>(context), ICategoryRepository
{
    private readonly ApplicationDbContext _dbContext = context;

    public Task<Category?> GetCategoryByIdAsync(
        CategoryId id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public Task<List<Category>> GetCategoryListAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Categories.AsNoTracking().ToListAsync(cancellationToken);
    }
}
