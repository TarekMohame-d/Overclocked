using Bogus;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Shared.ValueObjects;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.ValueObjects;

namespace Overclocked.Architecture.Tests.FakeData;

public sealed class ProductFaker : Faker<Product>
{
    public ProductFaker(Guid brandId, Guid categoryId)
    {
        CustomInstantiator(f =>
            Product.Create(
                id: ProductId.Create(),
                brandId: BrandId.Create(brandId),
                categoryId: CategoryId.Create(categoryId),
                name: $"{f.Company.CompanyName()}-{f.UniqueIndex}",
                description: f.Commerce.ProductDescription(),
                price: Money.Create(Math.Round(f.Random.Decimal(10m, 10_000m), 2)),
                discount: Money.Create(Math.Round(f.Random.Decimal(0m, 0.99m), 2)),
                stock: f.Random.Int(10, 100),
                thumbnail: $"https://res.cloudinary.com/over-clocked/brands/image.jpg",
                images: [],
                specifications: [],
                tags: []));
    }
}
