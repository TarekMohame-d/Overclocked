namespace Overclocked.Application.Abstraction.Services;

public interface IRefreshTokenHasher
{
    string Hash(string value);
    bool Verify(string value, string hash);
}
