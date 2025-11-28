using Application.Services.Brand.Mapping;
using Application.Services.Category.Mapping;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;
using Application.Services.Tag.DTOs.Response;
using Domain.Entities;
using ProductEntity = Domain.Entities.Product;

namespace Application.Services.Product.Mapping;

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
            IsDeleted = false,
        };

        product.TagProducts = request.Tags.Select(t =>
            new TagProduct
            {
                ProductId = product.Id,
                TagId = t
            }).ToList();

        product.ProductImages = request.Images?.Select(url =>
            new ProductImage
            {
                ProductId = product.Id,
                Image = url
            }).ToList() ?? [];

        product.Specifications = request.Specification.Select(s =>
            new Specification
            {
                ProductId = product.Id,
                Name = s.Name,
                Value = s.Value,
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
        entity.UpdatedAt = DateTime.UtcNow;

        entity.TagProducts.Reconcile(
            request.Tags,
            entityKey => entityKey.TagId,
            dtoKey => dtoKey,
            createFromDto: tagId => new TagProduct { ProductId = entity.Id, TagId = tagId });

        entity.ProductImages.Reconcile(
            request.Images ?? Enumerable.Empty<string>(),
            entityKey => entityKey.Image,
            dtoKey => dtoKey,
            createFromDto: url => new ProductImage { ProductId = entity.Id, Image = url });

        entity.Specifications.Reconcile(
            request.Specification,
            entityKey => entityKey.Name.Trim().ToUpperInvariant(),
            dtoKey => dtoKey.Name.Trim().ToUpperInvariant(),
            createFromDto: specDto => new Specification
            {
                ProductId = entity.Id,
                Name = specDto.Name,
                Value = specDto.Value
            },
            updateExisting: (entitySpec, dtoSpec) =>
            {
                entitySpec.Value = dtoSpec.Value;
            }
        );
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
            FinalPrice = Math.Round(x.Price * (1 - x.Discount), 2),
            Rating = x.Rating,
            ReviewCount = x.ReviewCount,
            Brand = x.Brand!.ToDto()
        });
    }
}
