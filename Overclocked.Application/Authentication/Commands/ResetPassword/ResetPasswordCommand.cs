namespace Overclocked.Application.Authentication.Commands.ResetPassword;

public record ResetPasswordCommand(string Email, string Password, string Code);
