using Overclocked.Application.Abstractions.Messaging;

namespace Overclocked.Application.Authentication.Commands.ResendEmailConfirmationCode;

public record ResendEmailConfirmationCodeCommand : ICommand
{
    public required string Email { get; init; }
}
