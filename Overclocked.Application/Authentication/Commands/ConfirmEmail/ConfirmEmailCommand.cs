namespace Overclocked.Application.Authentication.Commands.ConfirmEmail;

public record ConfirmEmailCommand
{
    public required string Email { get; init; }
    public required string Code { get; init; }
}
