using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Common.Enums;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Infrastructure.Persistence.Repositories;

public class ProductReadRepository(ApplicationDbContext dbContext) : IProductReadRepository
{
    private readonly IQueryable<Product> _queryable = dbContext.Products.AsNoTracking();

    public Task<int> CountAsync(
        string searchTerm,
        Guid brandId,
        Guid categoryId,
        Guid tagId,
        bool hasDiscount = false,
        CancellationToken ct = default
    )
    {
        IQueryable<Product> query = _queryable;

        query = ApplySearch(query, searchTerm);
        query = ApplyFilters(query, brandId, categoryId, tagId, hasDiscount);

        return query.CountAsync(ct);
    }

    public Task<List<TResult>> GetPagedAsync<TResult>(
        int pageNumber,
        int pageSize,
        string searchTerm,
        Guid brandId,
        Guid categoryId,
        Guid tagId,
        ProductSortField sortBy,
        SortDirection direction,
        bool hasDiscount,
        Expression<Func<Product, TResult>> selector,
        CancellationToken ct = default
    )
    {
        IQueryable<Product> query = _queryable;
        query = query.Include(p => p.Brand);

        query = ApplySearch(query, searchTerm);
        query = ApplyFilters(query, brandId, categoryId, tagId, hasDiscount);

        query = ApplySorting(query, sortBy, direction);

        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        return query.Select(selector).ToListAsync(ct);
    }

    public Task<TResult?> GetByIdAsync<TResult>(
        ProductId id,
        Expression<Func<Product, TResult>> selector,
        CancellationToken ct = default
    ) => _queryable.Where(p => p.Id == id).AsSplitQuery().Select(selector).FirstOrDefaultAsync(ct);

    public Task<List<Product>> GetByIdsAsync(List<ProductId> ids, CancellationToken ct = default)
    {
        IQueryable<Product> query = _queryable;

        return query.Where(p => ids.Contains(p.Id)).ToListAsync(ct);
    }

    public Task<bool> ExistsAsync(ProductId id, CancellationToken ct = default) => _queryable.AnyAsync(x => x.Id == id, ct);

    private static IQueryable<Product> ApplySearch(IQueryable<Product> query, string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return query;

        var normalizedTerm = searchTerm.ToUpper();
        return query.Where(p => p.NormalizedName.Contains(normalizedTerm) || p.Description.Contains(normalizedTerm));
    }

    private static IQueryable<Product> ApplyFilters(
        IQueryable<Product> query,
        Guid brandId,
        Guid categoryId,
        Guid tagId,
        bool hasDiscount = false
    )
    {
        if (hasDiscount)
            query = query.Where(p => p.Discount.Value > 0);

        if (brandId != Guid.Empty)
            query = query.Where(p => p.BrandId == BrandId.Create(brandId));

        if (categoryId != Guid.Empty)
            query = query.Where(p => p.CategoryId == CategoryId.Create(categoryId));

        if (tagId != Guid.Empty)
            query = query.Where(p => p.ProductTags.Any(pt => pt.TagId == TagId.Create(tagId)));

        return query;
    }

    private static IQueryable<Product> ApplySorting(IQueryable<Product> query, ProductSortField sortBy, SortDirection direction)
    {
        var isDescending = direction == SortDirection.Desc;

        return sortBy switch
        {
            ProductSortField.Name => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            ProductSortField.Price => isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            ProductSortField.Rating => isDescending
                ? query.OrderByDescending(p => p.ProductRating)
                : query.OrderBy(p => p.ProductRating),
            ProductSortField.Id or _ => isDescending ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
        };
    }
}
