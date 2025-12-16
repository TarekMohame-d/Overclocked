using Bogus;
using Overclocked.Domain.BrandAggregate;

namespace Overclocked.Architecture.Tests.FakeData;

public sealed class BrandFaker : Faker<Brand>
{
    public BrandFaker()
    {
        CustomInstantiator(f =>
            Brand.Create(
                $"{f.Company.CompanyName()}-{f.UniqueIndex}",
                $"https://res.cloudinary.com/over-clocked/brands/image.jpg"
            ));
    }
}
