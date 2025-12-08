using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class BrandRepository(ApplicationDbContext context)
    : GenericRepository<Brand, BrandId>(context), IBrandRepository
{
    private readonly ApplicationDbContext _dbContext = context;

    public Task<Brand?> GetBrandByIdAsync(
        BrandId id,
        CancellationToken cancellationToken = default)
    {
        return _dbContext.Brands.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public Task<List<Brand>> GetBrandListAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.Brands.AsNoTracking().ToListAsync(cancellationToken);
    }
}
