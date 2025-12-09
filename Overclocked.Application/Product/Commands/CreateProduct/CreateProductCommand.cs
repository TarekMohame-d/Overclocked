using Overclocked.Contracts.Category;
using Overclocked.Contracts.Product;
using static Overclocked.Application.Product.Commands.CreateProduct.CreateProductCommand;

namespace Overclocked.Application.Product.Commands.CreateProduct;

public record CreateProductCommand(
    Guid BrandId,
    Guid CategoryId,
    string Name,
    string Thumbnail,
    string Description,
    decimal Price,
    int StockQuantity,
    decimal? Discount,
    IEnumerable<Guid> Tags,
    IEnumerable<string>? Images,
    IEnumerable<Specs> Specification)
{
    public record Specs(string Name, string Value);

    public static CreateProductCommand Create(CreateProductRequest request)
    {
        return new(
            BrandId: request.BrandId,
            CategoryId: request.CategoryId,
            Name: request.Name,
            Thumbnail: request.Thumbnail,
            Description: request.Description,
            Price: request.Price,
            StockQuantity: request.StockQuantity,
            Discount: request.Discount,
            Tags: request.Tags,
            Images: request.Images,
            Specification: request.Specification.Select(x => new Specs(x.Name, x.Value))
        );
    }
}
