using Microsoft.Extensions.Logging;
using Overclocked.Application.Authentication.Commands.ConfirmEmail;
using Overclocked.Application.Authentication.Commands.ForgetPassword;
using Overclocked.Application.Authentication.Commands.Login;
using Overclocked.Application.Authentication.Commands.RefreshToken;
using Overclocked.Application.Authentication.Commands.Register;
using Overclocked.Application.Authentication.Commands.ResendEmailConfirmationCode;
using Overclocked.Application.Authentication.Commands.ResetPassword;
using Overclocked.Contracts.Authentication;
using Overclocked.Domain.Common.Results;
using Serilog.Context;

namespace Overclocked.Application.Authentication.Commands.Decorators;

public class LoggingAuthenticationCommandsDecorator(
    IAuthenticationCommands inner,
    ILogger<LoggingAuthenticationCommandsDecorator> logger) : IAuthenticationCommands
{
    public Task<Result> RegisterCommandHandler(RegisterCommand command, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(command, () => inner.RegisterCommandHandler(command, cancellationToken));

    public Task<Result> ConfirmEmailCommandHandler(ConfirmEmailCommand command, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(command, () => inner.ConfirmEmailCommandHandler(command, cancellationToken));

    public Task<Result<AuthResponse>> LoginCommandHandler(LoginCommand command, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(command, () => inner.LoginCommandHandler(command, cancellationToken));

    public Task<Result> ResendConfirmationCodeCommandHandler(
        ResendEmailConfirmationCodeCommand command,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(command, () => inner.ResendConfirmationCodeCommandHandler(
                command,
                cancellationToken));

    public Task<Result> ForgetPasswordCommandHandler(
        ForgetPasswordCommand command,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(command, () => inner.ForgetPasswordCommandHandler(command, cancellationToken));

    public Task<Result> ResetPasswordCommandHandler(
        ResetPasswordCommand command,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(command, () => inner.ResetPasswordCommandHandler(command, cancellationToken));

    public Task<Result<AuthResponse>> RefreshTokenCommandHandler(
        RefreshTokenCommand command,
        CancellationToken cancellationToken) =>
            ExecuteWithLoggingAsync(command, () => inner.RefreshTokenCommandHandler(command, cancellationToken));

    private async Task<TResult> ExecuteWithLoggingAsync<TResult>(object command, Func<Task<TResult>> action)
        where TResult : Result
    {
        var commandName = command as string ?? command.GetType().Name;
        logger.LogInformation("Processing command {CommandName}", commandName);

        TResult result = await action();

        if(result.IsSuccess)
        {
            logger.LogInformation("Completed command {CommandName}", commandName);
        }
        else
        {
            using(LogContext.PushProperty("Errors", result.Error, true))
            {
                logger.LogError("Completed command {@CommandName} with errors", commandName);
            }
        }

        return result;
    }
}
