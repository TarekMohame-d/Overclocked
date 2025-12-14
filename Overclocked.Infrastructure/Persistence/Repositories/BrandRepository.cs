using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class BrandRepository(ApplicationDbContext context)
    : GenericRepository<Brand, BrandId>(context), IBrandRepository
{
    public Task<Brand?> FindAsync(BrandId id, CancellationToken cancellationToken = default)
    {
        return _dbSet.FindAsync([id], cancellationToken: cancellationToken).AsTask();
    }

    public Task<Brand?> GetByIdAsync(
        BrandId id,
        CancellationToken cancellationToken = default)
    {
        return _dbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
    }

    public Task<List<Brand>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _dbSet.AsNoTracking().ToListAsync(cancellationToken);
    }
}
