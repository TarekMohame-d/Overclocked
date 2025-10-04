using Application.Features.Product.Commands.CreateProduct;
using Application.Features.Product.Queries.GetPagedProducts;
using ProductEntity = Domain.Entities.Product;

namespace Application.Features.Product.Mapping;

public static class ProductMapping
{
    public static ProductEntity ToEntity(this CreateProductCommand command)
    {
        return new ProductEntity
        {
            Name = command.Name,
            BrandId = command.BrandId,
            CategoryId = command.CategoryId,
            Description = command.Description,
            Discount = command.Discount,
            Price = command.Price,
            Thumbnail = command.Thumbnail,
            StockQuantity = command.Stock
        };
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
