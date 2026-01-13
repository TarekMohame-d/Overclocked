using Bogus;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.UserAggregate;

namespace Overclocked.Architecture.Tests.FakeData;

public sealed class UserFaker : Faker<User>
{
    private readonly IPasswordHasher _passwordHasher;

    public UserFaker(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
        CustomInstantiator(f =>
            User.Create(
                $"{f.Person.FirstName}",
                $"{f.Person.LastName}",
                $"{f.Internet.Email()}",
                GeneratePasswordHash(),
                GeneratePhone(),
                $"{f.Internet.Port()}/{f.UniqueIndex}",
                $"{f.Internet.Port()}/{f.UniqueIndex}"
            ).Value
        );
    }

    private string GeneratePasswordHash()
    {
        var password = "P@ssword123";
        return _passwordHasher.Hash(password);
    }

    private string GeneratePhone(int length = 15)
    {
        var random = new Random();
        var digits = new char[length];
        for (var i = 0; i < length; i++)
            digits[i] = (char)('0' + random.Next(10));

        return new string(digits);
    }
}
