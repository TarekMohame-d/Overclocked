using Bogus;
using Domain.Entities;

namespace ArchitectureTests.FakeData;

public sealed class CategoryFaker : Faker<Category>
{
    public CategoryFaker()
    {
        RuleFor(b => b.Id, f => Guid.CreateVersion7());
        RuleFor(c => c.Name, f => $"{f.Company.CompanyName()}-{f.UniqueIndex}");
        RuleFor(c => c.Image, f => $"{f.Image.PicsumUrl()}/{f.UniqueIndex}");
    }
}
