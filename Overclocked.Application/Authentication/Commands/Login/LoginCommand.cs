namespace Overclocked.Application.Authentication.Commands.Login;

public record LoginCommand
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string DeviceId { get; init; }
}
