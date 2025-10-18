using Application.Abstraction.Repositories;
using Application.Features.Brand.Queries.GetBrandById;
using Application.Features.Category.Queries.GetCategoryById;
using Application.Features.Product.Commands.Common.DTOs;
using Application.Features.Product.Queries.GetProductById;
using Application.Features.Tag.Queries.GetTagById;
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

    public async Task<ProductDto?> GetProductAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var productDto = await _dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == id)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Thumbnail = p.Thumbnail,
                Description = p.Description,
                Price = p.Price,
                Discount = p.Discount,
                Rating = p.Rating,
                Category = new CategoryDto
                {
                    Id = p.Category.Id,
                    Name = p.Category.Name,
                    ImageUrl = p.Category.Image
                },
                Brand = new BrandDto
                {
                    Id = p.Brand.Id,
                    Name = p.Brand.Name,
                    ImageUrl = p.Brand.Image
                },
                Images = p.ProductImages.Select(img => img.Image),
                Tags = p.TagProducts.Select(tp => new TagDto
                {
                    Id = tp.Tag.Id,
                    Name = tp.Tag.Name
                }),
                Specifications = p.Specifications.Select(ps => new SpecificationDto
                {
                    Id = ps.Id,
                    Name = ps.Name,
                    Value = ps.Value
                }),
                Reviews = p.Reviews.Select(r => new ProductReviewDto
                {
                    Id = r.Id,
                    Comment = r.Comment,
                    Rating = r.Rating,
                    CreatedAt = r.CreatedAt,
                    UserName = r.User.FirstName,
                    Reply = r.ReviewReply == null ? null : new ProductReviewReplyDto
                    {
                        Id = r.ReviewReply.Id,
                        Reply = r.ReviewReply.Reply,
                        CreatedAt = r.ReviewReply.CreatedAt
                    }
                })
            })
            .FirstOrDefaultAsync(cancellationToken);

        return productDto;
    }

    public IQueryable<Product> GetProductsQuery(string? sortBy)
    {
        IQueryable<Product> query = _dbContext.Products.AsNoTracking();

        if (string.IsNullOrWhiteSpace(sortBy))
            return query.OrderBy(p => p.Id);

        var parts = sortBy.Split('_');
        if (parts.Length != 2)
            return query.OrderBy(p => p.Id);

        var field = parts[0].ToLowerInvariant();
        var direction = parts[1].ToLowerInvariant();
        bool isDescending = direction == "desc";

        switch (field)
        {
            case "name":
                query = isDescending
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name);
                break;

            case "price":
                query = isDescending
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price);
                break;

            case "rating":
                query = isDescending
                    ? query.OrderByDescending(p => p.Rating)
                    : query.OrderBy(p => p.Rating);
                break;

            default:
                query = query.OrderBy(p => p.Id);
                break;
        }

        return query;
    }
}
