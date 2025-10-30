using Application.Features.Brand.Mapping;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;
using Domain.Entities;
using ProductEntity = Domain.Entities.Product;

namespace Application.Features.Product.Mapping;

public static class ProductMapping
{
    public static ProductEntity ToEntity(this CreateProductRequest request)
    {
        var product = new ProductEntity
        {
            Name = request.Name,
            BrandId = request.BrandId,
            CategoryId = request.CategoryId,
            Description = request.Description,
            Discount = request.Discount,
            Price = request.Price,
            Thumbnail = request.Thumbnail,
            StockQuantity = request.Stock,
        };

        product.TagProducts = request.Tags.Select(t => new TagProduct
        {
            ProductId = product.Id,
            TagId = t
        }).ToList();

        product.ProductImages = request.Images is not null
            ? request.Images.Select(url => new ProductImage
            {
                ProductId = product.Id,
                Image = url
            }).ToList()
            : null;

        product.Specifications = request.Specification.Select(s => new Specification
        {
            ProductId = product.Id,
            Name = s.Name,
            Value = s.Value
        }).ToList();

        return product;
    }

    public static void UpdateFrom(this ProductEntity entity, UpdateProductRequest request)
    {
        entity.BrandId = request.BrandId;
        entity.CategoryId = request.CategoryId;
        entity.Name = request.Name;
        entity.Thumbnail = request.Thumbnail;
        entity.Description = request.Description;
        entity.Price = request.Price;
        entity.StockQuantity = request.Stock;
        entity.Discount = request.Discount;

        entity.TagProducts = request.Tags.Select(t => new TagProduct
        {
            ProductId = entity.Id,
            TagId = t
        }).ToList();

        entity.ProductImages = request.Images is not null
            ? request.Images.Select(url => new ProductImage
            {
                ProductId = entity.Id,
                Image = url
            }).ToList()
            : null;

        entity.Specifications = request.Specification.Select(s => new Specification
        {
            ProductId = entity.Id,
            Name = s.Name,
            Value = s.Value
        }).ToList();

        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static IQueryable<ProductListResponse> ToDto(this IQueryable<ProductEntity> entities)
    {
        return entities.Select(x => new ProductListResponse
        {
            Id = x.Id,
            Name = x.Name,
            Thumbnail = x.Thumbnail,
            Price = x.Price,
            Discount = x.Discount,
            Rating = x.Rating,
            Brand = x.Brand.ToDto()
        });
    }
}
