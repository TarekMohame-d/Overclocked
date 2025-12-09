namespace Overclocked.Contracts.Authentication;

public record ResetPasswordRequest(string Email, string Code, string Password);
