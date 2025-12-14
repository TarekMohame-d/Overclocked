using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Contracts.Authentication;

namespace Overclocked.Application.Authentication.Commands.Login;

public record LoginCommand : ICommand<AuthResponse>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string DeviceId { get; init; }
}
