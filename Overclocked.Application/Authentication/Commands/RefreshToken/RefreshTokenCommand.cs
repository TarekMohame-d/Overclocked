namespace Overclocked.Application.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(string AccessToken, string RefreshToken);
