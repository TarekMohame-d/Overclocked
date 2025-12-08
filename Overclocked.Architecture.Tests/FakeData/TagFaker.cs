using Bogus;
using Overclocked.Domain.TagAggregate;
using Overclocked.Domain.TagAggregate.ValueObjects;

namespace Overclocked.Architecture.Tests.FakeData;

public sealed class TagFaker : Faker<Tag>
{
    public TagFaker()
    {
        CustomInstantiator(f =>
            Tag.Create(
                TagId.Create(),
                $"{f.Company.CompanyName()}-{f.UniqueIndex}"
            )
        );
    }
}
