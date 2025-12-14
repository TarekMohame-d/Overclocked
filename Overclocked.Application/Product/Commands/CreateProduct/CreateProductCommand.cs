using Overclocked.Application.Abstractions.Caching;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Contracts.Product;

namespace Overclocked.Application.Product.Commands.CreateProduct;

public record CreateProductCommand : ICommand, ICacheInvalidatorCommand
{
    public required Guid BrandId { get; init; }
    public required Guid CategoryId { get; init; }
    public required string Name { get; init; }
    public required string Thumbnail { get; init; }
    public required string Description { get; init; }
    public required decimal Price { get; init; }
    public required int StockQuantity { get; init; }
    public decimal? Discount { get; init; }
    public required IEnumerable<Guid> Tags { get; init; }
    public required IEnumerable<(string Name, string Value)> Specifications { get; init; }
    public IEnumerable<string>? Images { get; init; }

    public string[] CacheKeys => [];

    public string? CacheSetKey => Common.Constants.CacheKeys.ProductSet;

    public static CreateProductCommand Create(CreateProductRequest request)
    {
        return new()
        {
            BrandId = request.BrandId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            Thumbnail = request.Thumbnail,
            Description = request.Description,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            Discount = request.Discount,
            Tags = request.Tags,
            Images = request.Images,
            Specifications = request.Specifications.Select(x => (x.Name, x.Value))
        };
    }
}
