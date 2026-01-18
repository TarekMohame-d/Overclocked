using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class BrandRepository(ApplicationDbContext dbContext) : IBrandRepository
{
    private readonly DbSet<Brand> _dbSet = dbContext.Brands;

    public Task<Brand?> GetByIdAsync(BrandId id, CancellationToken ct = default) => _dbSet.FindAsync([id], ct).AsTask();

    public Task<bool> ExistsAsync(BrandId id, CancellationToken ct = default) => _dbSet.AnyAsync(x => x.Id == id, ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
    {
        var normalizedInput = name.Trim().ToUpperInvariant();
        return _dbSet.AnyAsync(x => x.NormalizedName == normalizedInput, ct);
    }

    public void Add(Brand brand) => _dbSet.Add(brand);

    public void Remove(Brand brand) => _dbSet.Remove(brand);
}
