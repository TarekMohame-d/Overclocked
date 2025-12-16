using Overclocked.Application.Product.Commands.CreateProduct;
using Overclocked.Contracts.Brand;
using Overclocked.Contracts.Product;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Shared.ValueObjects;
using Overclocked.Domain.ProductAggregate.Entities;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.TagAggregate.ValueObjects;
using ProductEntity = Overclocked.Domain.ProductAggregate.Product;

namespace Overclocked.Application.Product.Mapping;

public static class ProductMapper
{
    public static ProductEntity ToEntity(this CreateProductCommand command)
    {
        return ProductEntity.Create(
            id: ProductId.Create(),
            brandId: BrandId.Create(command.BrandId),
            categoryId: CategoryId.Create(command.CategoryId),
            name: command.Name,
            description: command.Description,
            thumbnail: command.Thumbnail,
            price: Money.Create(command.Price),
            discount: command.Discount is null ? Money.Zero : Money.Create((decimal)command.Discount),
            stock: command.StockQuantity,
            images: CreateProductImages(command.Images),
            specifications: CreateSpecifications(command.Specifications),
            tags: CreateProductTags(command.Tags)
        );
    }

    public static IEnumerable<ProductPagedResponse> ToDto(this List<ProductEntity> entities)
    {
        return entities.Select(x => new ProductPagedResponse
        {
            Id = x.Id,
            Name = x.Name,
            Thumbnail = x.Thumbnail,
            Price = x.Price.Amount,
            Discount = x.Discount.Amount,
            FinalPrice = x.CalculateFinalPrice(),
            Rating = x.ProductRating.AverageRating,
            ReviewCount = x.ProductRating.ReviewCount,
            Brand = new BrandResponse
            {
                Id = x.Brand!.Id,
                Name = x.Brand.Name,
                ImageUrl = x.Brand.ImageUrl
            }
        });
    }

    private static IEnumerable<Specification> CreateSpecifications(IEnumerable<(string Name, string Value)> specs)
    {
        return specs.Select(x => Specification.Create(x.Name, x.Value));
    }

    private static IEnumerable<ProductImage> CreateProductImages(IEnumerable<string>? images)
    {
        return images?.Select(ProductImage.Create) ?? [];
    }

    private static IEnumerable<ProductTag> CreateProductTags(IEnumerable<Guid> tags)
    {
        return tags.Select(x => ProductTag.Create(TagId.Create(x)));
    }
}
