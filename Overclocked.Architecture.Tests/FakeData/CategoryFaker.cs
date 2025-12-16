using Bogus;
using Overclocked.Domain.CategoryAggregate;

namespace Overclocked.Architecture.Tests.FakeData;

public sealed class CategoryFaker : Faker<Category>
{
    public CategoryFaker()
    {
        CustomInstantiator(f =>
            Category.Create(
                $"{f.Company.CompanyName()}-{f.UniqueIndex}",
                $"https://res.cloudinary.com/over-clocked/categories/image.jpg"
            )
        );
    }
}
