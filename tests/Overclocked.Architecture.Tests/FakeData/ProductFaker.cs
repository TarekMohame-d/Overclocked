using Bogus;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;
using Overclocked.Domain.Common.Shared.ValueObjects.Money;
using Overclocked.Domain.ProductAggregate;
using Overclocked.Domain.ProductAggregate.Entities;
using Overclocked.Domain.ProductAggregate.ValueObjects;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Architecture.Tests.FakeData;

public sealed class ProductFaker : Faker<Product>
{
    public ProductFaker(Guid brandId, Guid categoryId, List<Guid>? tags = null)
    {
        List<ProductTag> productTags = tags?.Select(x => ProductTag.Create(TagId.Create(x))).ToList() ?? [];

        if (productTags?.Any() != true)
            productTags = [ProductTag.Create(TagId.Create())];

        CustomInstantiator(f =>
            Product
                .Create(
                    brandId: BrandId.Create(brandId),
                    categoryId: CategoryId.Create(categoryId),
                    name: $"{f.Company.CompanyName()}-{f.UniqueIndex}",
                    description: f.Commerce.ProductDescription(),
                    thumbnail: Image.Create("https://res.cloudinary.com/over-clocked/brands/image.jpg").Value,
                    stock: f.Random.Int(10, 100),
                    price: Money.Create(Math.Round(f.Random.Decimal(10m, 10_000m), 2), "USD").Value,
                    discount: DiscountRate.Create(Math.Round(f.Random.Decimal(0m, 0.99m), 2)).Value,
                    images: [],
                    specifications: [Specification.Create("Name", "Value").Value],
                    productTags: productTags
                )
                .Value
        );
    }
}
