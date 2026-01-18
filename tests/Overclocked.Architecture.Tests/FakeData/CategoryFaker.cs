using Bogus;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.Common.Shared.ValueObjects.Image;

namespace Overclocked.Architecture.Tests.FakeData;

public sealed class CategoryFaker : Faker<Category>
{
    public CategoryFaker() =>
        CustomInstantiator(f =>
            Category
                .Create(
                    $"{f.Company.CompanyName()}-{f.UniqueIndex}",
                    Image.Create("https://res.cloudinary.com/over-clocked/brands/image.jpg").Value
                )
                .Value
        );
}
