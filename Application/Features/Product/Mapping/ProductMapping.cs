using Application.Features.Product.Commands.CreateProduct;
using Application.Features.Product.Commands.UpdateProduct;
using Application.Features.Product.Queries.GetPagedProducts;
using Domain.Entities;
using ProductEntity = Domain.Entities.Product;

namespace Application.Features.Product.Mapping;

public static class ProductMapping
{
    public static ProductEntity ToEntity(this CreateProductCommand command)
    {
        var product = new ProductEntity
        {
            Name = command.Name,
            BrandId = command.BrandId,
            CategoryId = command.CategoryId,
            Description = command.Description,
            Discount = command.Discount,
            Price = command.Price,
            Thumbnail = command.Thumbnail,
            StockQuantity = command.Stock,
        };

        product.TagProducts = command.Tags.Select(t => new TagProduct
        {
            ProductId = product.Id,
            TagId = t
        }).ToList();

        product.ProductImages = command.Images is not null
            ? command.Images.Select(url => new ProductImage
            {
                ProductId = product.Id,
                Image = url
            }).ToList()
            : [];

        product.Specifications = command.Specification.Select(s => new Specification
        {
            ProductId = product.Id,
            Name = s.Name,
            Value = s.Value
        }).ToList();

        return product;
    }

    public static void UpdateFrom(this ProductEntity entity, UpdateProductWithIdCommand command)
    {
        entity.BrandId = command.BrandId;
        entity.CategoryId = command.CategoryId;
        entity.Name = command.Name;
        entity.Thumbnail = command.Thumbnail;
        entity.Description = command.Description;
        entity.Price = command.Price;
        entity.StockQuantity = command.Stock;
        entity.Discount = command.Discount;

        entity.TagProducts = command.Tags.Select(t => new TagProduct
        {
            ProductId = entity.Id,
            TagId = t
        }).ToList();

        entity.ProductImages = command.Images is not null
            ? command.Images.Select(url => new ProductImage
            {
                ProductId = entity.Id,
                Image = url
            }).ToList()
            : [];

        entity.Specifications = command.Specification.Select(s => new Specification
        {
            ProductId = entity.Id,
            Name = s.Name,
            Value = s.Value
        }).ToList();

        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static IQueryable<ProductListDto> ToDto(this IQueryable<ProductEntity> entities)
    {
        return entities.Select(x => new ProductListDto
        {
            Id = x.Id,
            Name = x.Name,
            Thumbnail = x.Thumbnail,
            Price = x.Price,
            Discount = x.Discount,
            Rating = x.Rating
        });
    }
}
