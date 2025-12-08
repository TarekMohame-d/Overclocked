namespace Overclocked.Application.Authentication.Commands.Login;

public record LoginCommand(string Email, string Password, string DeviceId);
