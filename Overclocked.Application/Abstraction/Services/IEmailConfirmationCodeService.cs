namespace Overclocked.Application.Abstraction.Services;

public interface IEmailConfirmationCodeService
{
    string Hash(string value);
    bool Verify(string value, string hash);
    string GenerateVerificationCode(int length = 6);
}
