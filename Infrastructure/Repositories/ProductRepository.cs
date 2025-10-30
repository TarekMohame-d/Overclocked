using Application.Abstraction.Repositories;
using Application.Common.Enums;
using Application.Services.Brand.DTOs.Response;
using Application.Services.Category.DTOs.Response;
using Application.Services.Product.DTOs.Response;
using Application.Services.Tag.DTOs.Response;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Product?> GetByIdWithImagesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products
            .Include(p => p.ProductImages)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<ProductResponse?> GetProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var productDto = await _dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductResponse
            {
                Id = p.Id,
                Name = p.Name,
                Thumbnail = p.Thumbnail,
                Description = p.Description,
                Price = p.Price,
                Discount = p.Discount,
                Rating = p.Rating,
                Category = new CategoryResponse
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name,
                    ImageUrl = p.Category.Image
                },
                Brand = new BrandResponse
                {
                    Id = p.Brand.Id,
                    Name = p.Brand.Name,
                    ImageUrl = p.Brand.Image
                },
                Images = p.ProductImages == null ? null : p.ProductImages.Select(img => img.Image),
                Tags = p.TagProducts.Select(tp => new TagResponse
                {
                    Id = tp.Tag.Id,
                    Name = tp.Tag.Name
                }),
                Specifications = p.Specifications.Select(ps => new ProductSpecificationResponse
                {
                    Id = ps.Id,
                    Name = ps.Name,
                    Value = ps.Value
                }),
                // Reviews = p.Reviews.Select(r => new ProductReviewDto
                // {
                //     Id = r.Id,
                //     Comment = r.Comment,
                //     Rating = r.Rating,
                //     CreatedAt = r.CreatedAt,
                //     UserName = r.User.FirstName,
                //     Reply = r.ReviewReply == null ? null : new ProductReviewReplyDto
                //     {
                //         Id = r.ReviewReply.Id,
                //         Reply = r.ReviewReply.Reply,
                //         CreatedAt = r.ReviewReply.CreatedAt
                //     }
                // })
            })
            .FirstOrDefaultAsync(cancellationToken);

        return productDto;
    }

    public IQueryable<Product> GetProductsQuery(
        ProductSortField sortBy,
        SortDirection direction,
        string? search = null,
        string? category = null,
        string? brand = null,
        Guid? tagId = null)
    {
        var query = _dbContext.Products
            .Include(p => p.Brand)
            .Include(p => p.Category)
            .AsNoTracking();

        query = ApplyFilters(query, search, category, brand, tagId);
        query = ApplySorting(query, sortBy, direction);

        return query;
    }

    private IQueryable<Product> ApplyFilters(
        IQueryable<Product> query,
        string? search,
        string? category,
        string? brand,
        Guid? tagId)
    {
        // Apply search filter (searches in product name and description)
        if (!string.IsNullOrWhiteSpace(search))
        {
            var sanitizedSearch = EscapeLikePattern(search);
            query = query.Where(p =>
                EF.Functions.ILike(p.Name, $"%{sanitizedSearch}%") ||
                EF.Functions.ILike(p.Description, $"%{sanitizedSearch}%"));
        }

        // Apply category filter
        if (!string.IsNullOrWhiteSpace(category))
        {
            var sanitizedCategory = EscapeLikePattern(category);
            query = query.Where(p => EF.Functions.ILike(p.Category.Name, $"%{sanitizedCategory}%"));
        }

        // Apply brand filter
        if (!string.IsNullOrWhiteSpace(brand))
        {
            var sanitizedBrand = EscapeLikePattern(brand);
            query = query.Where(p => EF.Functions.ILike(p.Brand.Name, $"%{sanitizedBrand}%"));
        }

        if (tagId != null)
            query = query.Where(p => p.TagProducts.Any(tp => tp.TagId == tagId));

        return query;
    }

    private IQueryable<Product> ApplySorting(
        IQueryable<Product> query,
        ProductSortField sortBy,
        SortDirection direction)
    {
        var isDescending = direction == SortDirection.Desc;

        return sortBy switch
        {
            ProductSortField.Name => isDescending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),

            ProductSortField.Price => isDescending
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),

            ProductSortField.Rating => isDescending
                ? query.OrderByDescending(p => p.Rating)
                : query.OrderBy(p => p.Rating),

            ProductSortField.Id or _ => isDescending
                ? query.OrderByDescending(p => p.Id)
                : query.OrderBy(p => p.Id)
        };
    }

    private static string EscapeLikePattern(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return input
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_")
            .Replace("[", "\\[");
    }
}
