using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Authentication.Commands.ResetPassword;

public record ResetPasswordCommand : ICommand
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string Code { get; init; }
}
