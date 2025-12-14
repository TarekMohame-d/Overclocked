using Bogus;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.Common.StaticData;
using Overclocked.Domain.RoleAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Architecture.Tests.FakeData;

public sealed class UserFaker : Faker<User>
{
    private readonly IPasswordHasher _passwordHasher;
    public UserFaker(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
        CustomInstantiator(f =>
            User.Create(
                UserId.Create(),
                RoleId.Create((int)RoleType.Customer),
                $"{f.Person.FirstName}-{f.UniqueIndex}",
                $"{f.Person.LastName}/{f.UniqueIndex}",
                $"{f.Internet.Email()}",
                GeneratePasswordHash(),
                $"{f.Phone.PhoneNumber()}/{f.UniqueIndex}",
                $"{f.Internet.Port()}/{f.UniqueIndex}",
                $"{f.Internet.Port()}/{f.UniqueIndex}"
            ));
    }

    private string GeneratePasswordHash()
    {
        var password = "P@ssword123";
        return _passwordHasher.Hash(password);
    }
}
