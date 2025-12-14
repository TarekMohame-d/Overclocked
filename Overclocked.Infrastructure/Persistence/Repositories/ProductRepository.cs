using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class ProductRepository(ApplicationDbContext context)
    : GenericRepository<Product, ProductId>(context), IProductRepository
{
    public Task<List<Product>> GetByIdsAsync(List<ProductId> ids, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .AsNoTracking()
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<Product?> GetByIdAsync(ProductId id, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .AsNoTracking()
            .AsSplitQuery()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Specifications)
            .Include(p => p.Tags)
                .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<Product?> GetForUpdateAsync(ProductId id, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .AsTracking()
            .AsSplitQuery()
            .Include(p => p.Images)
            .Include(p => p.Specifications)
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public Task<Product?> FindAsync(ProductId id, CancellationToken cancellationToken = default)
    {
        return _dbSet.FindAsync([id], cancellationToken: cancellationToken).AsTask();
    }

    public Task<int> CountAsync(
        string searchTerm,
        Guid brandId,
        Guid categoryId,
        Guid tagId,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = _dbContext.Products.AsNoTracking();

        query = ApplySearch(query, searchTerm);
        query = ApplyFilters(query, brandId, categoryId, tagId);

        return query.CountAsync(cancellationToken);
    }

    public Task<List<Product>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string searchTerm,
        Guid brandId,
        Guid categoryId,
        Guid tagId,
        ProductSortField sortBy,
        SortDirection direction,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = _dbSet.AsNoTracking();
        query = query.Include(p => p.Brand);

        query = ApplySearch(query, searchTerm);
        query = ApplyFilters(query, brandId, categoryId, tagId);

        query = ApplySorting(query, sortBy, direction);

        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return query.ToListAsync(cancellationToken);
    }

    private static IQueryable<Product> ApplySearch(IQueryable<Product> query, string searchTerm)
    {
        if(string.IsNullOrWhiteSpace(searchTerm))
        {
            return query;
        }

        var normalizedTerm = searchTerm.ToUpper();
        return query.Where(p => p.NormalizedName.Contains(normalizedTerm) ||
                                p.Description.Contains(normalizedTerm));
    }

    private static IQueryable<Product> ApplyFilters(
        IQueryable<Product> query,
        Guid brandId,
        Guid categoryId,
        Guid tagId)
    {
        if(brandId != Guid.Empty)
        {
            query = query.Where(p => p.BrandId == BrandId.Create(brandId));
        }

        if(categoryId != Guid.Empty)
        {
            query = query.Where(p => p.CategoryId == CategoryId.Create(categoryId));
        }

        if(tagId != Guid.Empty)
        {
            query = query.Where(p => p.Tags.Any(pt => pt.TagId == TagId.Create(tagId)));
        }

        return query;
    }

    private static IQueryable<Product> ApplySorting(
        IQueryable<Product> query,
        ProductSortField sortBy,
        SortDirection direction)
    {
        var isDescending = direction == SortDirection.Desc;

        return sortBy switch
        {
            ProductSortField.Name => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            ProductSortField.Price => isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            ProductSortField.Rating => isDescending ? query.OrderByDescending(p => p.ProductRating) : query.OrderBy(p => p.ProductRating),
            ProductSortField.Id or _ => isDescending ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
        };
    }
}
