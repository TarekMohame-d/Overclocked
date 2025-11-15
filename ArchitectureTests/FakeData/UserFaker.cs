using Bogus;
using Domain.Entities;
using Domain.StaticData;

namespace ArchitectureTests.FakeData;

public sealed class UserFaker : Faker<User>
{
    public UserFaker()
    {
        RuleFor(b => b.Id, f => Guid.CreateVersion7());
        RuleFor(b => b.FirstName, f => $"{f.Name.FirstName()}-{f.UniqueIndex}");
        RuleFor(b => b.LastName, f => $"{f.Name.LastName()}/{f.UniqueIndex}");
        RuleFor(b => b.Email, f => $"{f.Internet.Email()}");
        RuleFor(b => b.PasswordHash, f => $"{f.Internet.Password()}");
        RuleFor(b => b.EmailConfirmed, true);
        RuleFor(b => b.Phone, f => $"{f.Phone.PhoneNumber()}/{f.UniqueIndex}");
        RuleFor(b => b.RoleType, f => RoleType.Customer);
    }
}
