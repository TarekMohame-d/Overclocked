using Bogus;
using Overclocked.Domain.BrandAggregate;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;

namespace Overclocked.Architecture.Tests.FakeData;

public sealed class BrandFaker : Faker<Brand>
{
    public BrandFaker() =>
        CustomInstantiator(f =>
            Brand
                .Create(
                    $"{f.Company.CompanyName()}-{f.UniqueIndex}",
                    Image.Create("https://res.cloudinary.com/over-clocked/brands/image.jpg").Value
                )
                .Value
        );
}
