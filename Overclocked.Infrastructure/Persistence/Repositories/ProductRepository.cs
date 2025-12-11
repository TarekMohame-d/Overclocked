using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class ProductRepository(ApplicationDbContext context)
    : GenericRepository<Product, ProductId>(context), IProductRepository
{
    private readonly ApplicationDbContext _dbContext = context;
    public Task<Product?> GetByIdWithDetailsAsync(ProductId id, CancellationToken cancellationToken)
    {
        return _dbContext.Products
            .AsNoTracking()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Specifications)
            .Include(p => p.Tags)
                .ThenInclude(pt => pt.Tag)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }
}
