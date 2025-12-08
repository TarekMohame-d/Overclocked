using Bogus;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.BrandAggregate.ValueObjects;

namespace Overclocked.Architecture.Tests.FakeData;

public sealed class BrandFaker : Faker<Brand>
{
    public BrandFaker()
    {
        CustomInstantiator(f =>
            Brand.Create(
                BrandId.Create(),
                $"{f.Company.CompanyName()}-{f.UniqueIndex}",
                $"https://res.cloudinary.com/over-clocked/brands/image.jpg"
            ));
    }
}
