namespace Overclocked.Application.Authentication.Commands.ResendEmailConfirmationCode;

public record ResendEmailConfirmationCodeCommand
{
    public required string Email { get; init; }
}
