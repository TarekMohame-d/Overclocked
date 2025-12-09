namespace Overclocked.Application.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
