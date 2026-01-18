using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class CategoryReadRepository(ApplicationDbContext dbContext) : ICategoryReadRepository
{
    private readonly IQueryable<Category> _queryable = dbContext.Categories.AsNoTracking();

    public Task<List<Category>> GetAllAsync(CancellationToken ct = default) => _queryable.ToListAsync(ct);

    public Task<Category?> GetByIdAsync(CategoryId id, CancellationToken ct = default) =>
        _queryable.FirstOrDefaultAsync(x => x.Id == id, ct);
}
