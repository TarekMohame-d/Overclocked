using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Authentication.Commands.ForgetPassword;

public record ForgetPasswordCommand : ICommand
{
    public required string Email { get; init; }
}
