using Bogus;
using Overclocked.Domain.CategoryAggregate;
using Overclocked.Domain.CategoryAggregate.ValueObjects;

namespace Overclocked.Architecture.Tests.FakeData;

public sealed class CategoryFaker : Faker<Category>
{
    public CategoryFaker()
    {
        CustomInstantiator(f =>
            Category.Create(
                CategoryId.Create(),
                $"{f.Company.CompanyName()}-{f.UniqueIndex}",
                $"https://res.cloudinary.com/over-clocked/categories/image.jpg"
            )
        );
    }
}
