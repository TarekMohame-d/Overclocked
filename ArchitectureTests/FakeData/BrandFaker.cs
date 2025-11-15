using Bogus;
using Domain.Entities;

namespace ArchitectureTests.FakeData;

public sealed class BrandFaker : Faker<Brand>
{
    public BrandFaker()
    {
        RuleFor(b => b.Id, f => Guid.CreateVersion7());
        RuleFor(b => b.Name, f => $"{f.Company.CompanyName()}-{f.UniqueIndex}");
        RuleFor(b => b.Image, f => $"{f.Image.PicsumUrl()}/{f.UniqueIndex}");
    }
}
