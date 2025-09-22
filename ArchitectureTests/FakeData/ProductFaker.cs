using Bogus;
using Domain.Entities;

namespace ArchitectureTests.FakeData;

public class ProductFaker : Faker<Product>
{
    public ProductFaker()
    {
        RuleFor(p => p.Name, f => $"{f.Commerce.ProductName()}-{f.UniqueIndex}");
        RuleFor(p => p.Thumbnail, f => $"{f.Image.PicsumUrl()}/{f.UniqueIndex}");
        RuleFor(p => p.Description, f => f.Commerce.ProductDescription());
        RuleFor(p => p.Price, f => Math.Round(f.Random.Decimal(10m, 10_000m), 2));
        RuleFor(p => p.Discount, f => Math.Round(f.Random.Decimal(0m, 0.99m), 2));
        RuleFor(p => p.Rating, f => Math.Round(f.Random.Decimal(0m, 5m), 1));
        RuleFor(p => p.StockQuantity, f => f.Random.Int(10, 100));
    }
}
