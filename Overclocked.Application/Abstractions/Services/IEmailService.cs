namespace Overclocked.Application.Abstractions.Services;

public interface IEmailService
{
    Task SendConfirmationCode(string to, string code);
}
