namespace Application.Abstraction.Services;

public interface IHasher
{
    string Hash(string value);
    bool Verify(string value, string hash);
}

public interface IPasswordHasher : IHasher { }

public interface IEmailConfirmationCodeHasher : IHasher { }

public interface IRefreshTokenHasher : IHasher { }
