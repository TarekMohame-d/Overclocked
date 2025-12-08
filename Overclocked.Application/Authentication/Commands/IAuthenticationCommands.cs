using Overclocked.Application.Authentication.Commands.ConfirmEmail;
using Overclocked.Application.Authentication.Commands.ForgetPassword;
using Overclocked.Application.Authentication.Commands.Login;
using Overclocked.Application.Authentication.Commands.RefreshToken;
using Overclocked.Application.Authentication.Commands.Register;
using Overclocked.Application.Authentication.Commands.ResendEmailConfirmationCode;
using Overclocked.Application.Authentication.Commands.ResetPassword;
using Overclocked.Contracts.Authentication;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Authentication.Commands;

public interface IAuthenticationCommands
{
    Task<Result> RegisterCommandHandler(RegisterCommand command, CancellationToken cancellationToken);
    Task<Result> ConfirmEmailCommandHandler(ConfirmEmailCommand command, CancellationToken cancellationToken);
    Task<Result<AuthResponse>> LoginCommandHandler(LoginCommand command, CancellationToken cancellationToken);
    Task<Result> ResendConfirmationCodeCommandHandler(
        ResendEmailConfirmationCodeCommand command,
        CancellationToken cancellationToken);

    Task<Result> ForgetPasswordCommandHandler(ForgetPasswordCommand command, CancellationToken cancellationToken);
    Task<Result> ResetPasswordCommandHandler(ResetPasswordCommand command, CancellationToken cancellationToken);
    Task<Result<AuthResponse>> RefreshTokenCommandHandler(
        RefreshTokenCommand command,
        CancellationToken cancellationToken);
}
