using Bogus;
using Domain.Entities;

namespace ArchitectureTests.FakeData;

public sealed class EmailConfirmationCodeFaker : Faker<EmailConfirmationCode>
{
    public EmailConfirmationCodeFaker()
    {
        RuleFor(x => x.Id, f => Guid.CreateVersion7());
        RuleFor(x => x.UserId, f => Guid.CreateVersion7());
        RuleFor(x => x.CodeHash, f => $"{f.Random.Hash()}/{f.UniqueIndex}");
        RuleFor(x => x.IsUsed, f => false);
        RuleFor(x => x.ExpiredAt, DateTime.Now.AddMinutes(10));
    }
}
