namespace Overclocked.Application.Authentication.Commands.ConfirmEmail;

public record ConfirmEmailCommand(string Email, string Code);
