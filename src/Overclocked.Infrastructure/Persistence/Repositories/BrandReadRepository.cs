using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class BrandReadRepository(ApplicationDbContext dbContext) : IBrandReadRepository
{
    private readonly IQueryable<Brand> _queryable = dbContext.Brands.AsNoTracking();

    public Task<List<Brand>> GetAllAsync(CancellationToken ct = default) => _queryable.ToListAsync(ct);

    public Task<Brand?> GetByIdAsync(BrandId id, CancellationToken ct = default) =>
        _queryable.FirstOrDefaultAsync(x => x.Id == id, ct);
}
