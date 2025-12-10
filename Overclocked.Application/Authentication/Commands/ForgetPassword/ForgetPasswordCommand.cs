namespace Overclocked.Application.Authentication.Commands.ForgetPassword;

public record ForgetPasswordCommand
{
    public required string Email { get; init; }
}
