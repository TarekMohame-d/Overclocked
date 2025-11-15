namespace Application.Abstraction.Services;

public interface IEmailService
{
    Task SendConfirmationCode(string to, string code);
}
