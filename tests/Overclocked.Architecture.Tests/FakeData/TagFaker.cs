using Bogus;
using Overclocked.Domain.TagAggregate;

namespace Overclocked.Architecture.Tests.FakeData;

public sealed class TagFaker : Faker<Tag>
{
    public TagFaker() => CustomInstantiator(f => Tag.Create($"{f.Company.CompanyName()}-{f.UniqueIndex}").Value);
}
