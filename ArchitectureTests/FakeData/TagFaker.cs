using Bogus;
using Domain.Entities;

namespace ArchitectureTests.FakeData;

public class TagFaker : Faker<Tag>
{
    public TagFaker()
    {
        RuleFor(b => b.Id, f => Guid.CreateVersion7());
        RuleFor(b => b.Name, f => $"{Guid.NewGuid()}");
    }
}
