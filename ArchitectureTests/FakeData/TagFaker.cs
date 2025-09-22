using Bogus;
using Domain.Entities;

namespace ArchitectureTests.FakeData;

public class TagFaker : Faker<Tag>
{
    public TagFaker()
    {
        RuleFor(b => b.Name, f => $"{Guid.NewGuid()}");
    }
}
