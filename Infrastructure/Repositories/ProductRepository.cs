using Application.Abstraction.Repositories;
using Application.Common.Enums;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(ApplicationDbContext dbContext)
    : GenericRepository<Product>(dbContext), IProductRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public async Task<Product?> GetProductForUpdateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .AsTracking()
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.TagProducts)
            .Include(p => p.Specifications)
            .Include(p => p.ProductImages)
            .AsSplitQuery()
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product?> GetProductWithImagesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Include(p => p.ProductImages)
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public IQueryable<Product> GetProductsQuery(
        ProductSortField sortBy,
        SortDirection direction,
        string? search = null,
        string? category = null,
        string? brand = null,
        Guid? tagId = null)
    {
        IQueryable<Product> query = _dbContext.Products.AsNoTracking();

        query = ApplyFilters(query, search, category, brand, tagId);
        query = ApplySorting(query, sortBy, direction);

        return query;
    }

    private static IQueryable<Product> ApplyFilters(
        IQueryable<Product> query,
        string? search,
        string? category,
        string? brand,
        Guid? tagId)
    {
        // Apply search filter (searches in product name and description)
        if(!string.IsNullOrWhiteSpace(search))
        {
            var sanitizedSearch = EscapeLikePattern(search);
            query = query.Where(p =>
                EF.Functions.ILike(p.Name, $"%{sanitizedSearch}%")
                || EF.Functions.ILike(p.Description, $"%{sanitizedSearch}%"));
        }

        // Apply category filter
        if(!string.IsNullOrWhiteSpace(category))
        {
            var sanitizedCategory = EscapeLikePattern(category);
            query = query.Where(p => EF.Functions.ILike(p.Category!.Name, $"%{sanitizedCategory}%"));
        }

        // Apply brand filter
        if(!string.IsNullOrWhiteSpace(brand))
        {
            var sanitizedBrand = EscapeLikePattern(brand);
            query = query.Where(p => EF.Functions.ILike(p.Brand!.Name, $"%{sanitizedBrand}%"));
        }

        if(tagId != null)
        {
            query = query.Where(p => p.TagProducts.Any(tp => tp.TagId == tagId));
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

            ProductSortField.Price => isDescending
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),

            ProductSortField.Rating => isDescending
                ? query.OrderByDescending(p => p.Rating)
                : query.OrderBy(p => p.Rating),

            ProductSortField.Id or _ => isDescending ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
        };
    }

    private static string EscapeLikePattern(string input) =>
        input.Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_").Replace("[", "\\[");
}
