using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.DTOs.Response;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Application.Services.Authentication.Decorators;

public class LoggingAuthenticationServiceDecorator(
    IAuthenticationService inner,
    ILogger<LoggingAuthenticationServiceDecorator> logger)
    : IAuthenticationService
{
    public Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.RegisterAsync(request, cancellationToken));

    public Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.ConfirmEmailAsync(request, cancellationToken));

    public Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.LoginAsync(request, cancellationToken));

    public Task<Result> ResendEmailConfirmationCodeAsync(ResendEmailConfirmationCodeRequest request,
        CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.ResendEmailConfirmationCodeAsync(request, cancellationToken));

    public Task<Result> ForgetPasswordAsync(ForgetPasswordRequest request, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.ForgetPasswordAsync(request, cancellationToken));

    public Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken) =>
        ExecuteWithLoggingAsync(request, () => inner.ResetPasswordAsync(request, cancellationToken));

    public Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken) =>
    ExecuteWithLoggingAsync(request, () => inner.RefreshTokenAsync(request, cancellationToken));

    private async Task<TResult> ExecuteWithLoggingAsync<TResult>(
        object request,
        Func<Task<TResult>> action)
        where TResult : Result
    {
        var requestName = request.GetType().Name;
        logger.LogInformation("Processing request {RequestName}", requestName);

        TResult result = await action();

        if (result.IsSuccess)
            logger.LogInformation("Completed request {RequestName}", requestName);
        else
        {
            using (LogContext.PushProperty("Errors", result.Error, true))
                logger.LogError("Completed request {@RequestName} with errors", requestName);
        }

        return result;
    }
}
