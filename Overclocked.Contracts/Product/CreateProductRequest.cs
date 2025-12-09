using static Overclocked.Contracts.Product.CreateProductRequest;

namespace Overclocked.Contracts.Product;

public record CreateProductRequest(
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
}
