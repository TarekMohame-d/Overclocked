using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Contracts.Authentication;

namespace Overclocked.Application.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand : ICommand<AuthResponse>
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
}
