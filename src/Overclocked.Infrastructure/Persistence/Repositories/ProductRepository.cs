using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
{
    private readonly DbSet<Product> _dbSet = dbContext.Products;

    public Task<Product?> GetByIdAsync(ProductId id, CancellationToken ct = default) =>
        _dbSet
            .AsTracking()
            .AsSplitQuery()
            .Include(p => p.Images)
            .Include(p => p.Specifications)
            .Include(p => p.ProductTags)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<List<Product>> GetByIdsAsync(List<ProductId> ids, CancellationToken ct = default) =>
        _dbSet.AsTracking().Where(p => ids.Contains(p.Id)).ToListAsync(ct);

    public Task<bool> ExistsAsync(ProductId id, CancellationToken ct = default) => _dbSet.AnyAsync(x => x.Id == id, ct);

    public Task<bool> NameExistsAsync(string name, CancellationToken ct = default)
    {
        var normalizedInput = name.Trim().ToUpperInvariant();
        return _dbSet.AnyAsync(x => x.NormalizedName == normalizedInput, ct);
    }

    public void Add(Product product) => _dbSet.Add(product);

    public void Remove(Product product) => _dbSet.Remove(product);

    public Task<Product?> FindAsync(ProductId id, CancellationToken ct = default) => _dbSet.FindAsync([id], ct).AsTask();
}
