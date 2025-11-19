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
            Rating = 0,
            IsDeleted = false,
        };

        product.TagProducts = request.Tags.Select(t => new TagProduct { ProductId = product.Id, TagId = t }).ToList();

        product.ProductImages =
            request.Images?.Select(url => new ProductImage { ProductId = product.Id, Image = url }).ToList() ?? [];

        product.Specifications = request
            .Specification.Select(s => new Specification
            {
                ProductId = product.Id,
                Name = s.Name,
                Value = s.Value,
            })
            .ToList();

        return product;
    }

    public static ProductResponse ToDto(this ProductEntity entity)
    {
        return new ProductResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Thumbnail = entity.Thumbnail,
            Price = entity.Price,
            Discount = entity.Discount,
            Rating = entity.Rating,
            Category = entity.Category!.ToDto(),
            Brand = entity.Brand!.ToDto(),
            Specifications = entity.Specifications.Select(s => new ProductSpecificationResponse
            {
                Id = s.Id,
                Name = s.Name,
                Value = s.Value,
            }),
            Images = entity.ProductImages.Select(pImage => pImage.Image),
            Tags = entity.TagProducts.Select(tp => new TagResponse { Id = tp.TagId, Name = tp.Tag!.Name }).ToList(),
        };
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

        UpdateProductTags(entity, request.Tags);
        UpdateProductImages(entity, request.Images);
        UpdateProductSpecifications(entity, request.Specification);

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
            Brand = x.Brand!.ToDto(),
        });
    }

    private static void UpdateProductTags(ProductEntity product, IEnumerable<Guid> tags)
    {
        var tagsToBeRemoved = product.TagProducts.Where(tp => !tags.Contains(tp.TagId)).ToList();
        foreach(TagProduct tagProduct in tagsToBeRemoved)
            product.TagProducts.Remove(tagProduct);

        foreach(Guid tag in tags)
        {
            if(!product.TagProducts.Any(tp => tp.TagId == tag))
            {
                product.TagProducts.Add(new TagProduct { ProductId = product.Id, TagId = tag });
            }
        }
    }

    private static void UpdateProductImages(ProductEntity product, IEnumerable<string>? images)
    {
        if(images is null || !images.Any())
        {
            product.ProductImages.Clear();
            return;
        }

        var imagesToBeRemoved = product.ProductImages.Where(pImage => !images.Contains(pImage.Image)).ToList();
        foreach(ProductImage productImage in imagesToBeRemoved)
            product.ProductImages.Remove(productImage);

        foreach(var image in images)
        {
            if(!product.ProductImages.Any(pImage => pImage.Image == image))
            {
                product.ProductImages.Add(new ProductImage { ProductId = product.Id, Image = image });
            }
        }
    }

    private static void UpdateProductSpecifications(
        ProductEntity product,
        IEnumerable<UpdateProductRequest.Specs> specs
    )
    {
        var incomingKeys = specs.Select(s => s.Name.Trim().ToUpperInvariant()).ToHashSet();

        var specsToRemove = product
            .Specifications.Where(s => !incomingKeys.Contains(s.Name.Trim().ToUpperInvariant()))
            .ToList();

        foreach(Specification spec in specsToRemove)
            product.Specifications.Remove(spec);

        foreach(UpdateProductRequest.Specs specDto in specs)
        {
            Specification? existingSpec = product.Specifications.FirstOrDefault(s =>
                s.Name.Equals(specDto.Name, StringComparison.CurrentCultureIgnoreCase)
            );

            if(existingSpec is null)
            {
                product.Specifications.Add(
                    new Specification
                    {
                        ProductId = product.Id,
                        Name = specDto.Name,
                        Value = specDto.Value,
                    }
                );
            }
            else
            {
                existingSpec.Value = specDto.Value;
            }
        }
    }
}
