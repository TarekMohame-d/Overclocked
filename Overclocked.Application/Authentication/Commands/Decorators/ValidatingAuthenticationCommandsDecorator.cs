using FluentValidation;
using FluentValidation.Results;
using Overclocked.Application.Authentication.Commands.ConfirmEmail;
using Overclocked.Application.Authentication.Commands.ForgetPassword;
using Overclocked.Application.Authentication.Commands.Login;
using Overclocked.Application.Authentication.Commands.RefreshToken;
using Overclocked.Application.Authentication.Commands.Register;
using Overclocked.Application.Authentication.Commands.ResendEmailConfirmationCode;
using Overclocked.Application.Authentication.Commands.ResetPassword;
using Overclocked.Contracts.Authentication;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Application.Authentication.Commands.Decorators;

public class ValidatingAuthenticationCommandsDecorator(IAuthenticationCommands inner,
        IValidator<RegisterCommand> createValidator,
        IValidator<ConfirmEmailCommand> confirmEmailValidator,
        IValidator<LoginCommand> loginValidator,
        IValidator<ResendEmailConfirmationCodeCommand> resendEmailConfirmationCodeValidator,
        IValidator<ForgetPasswordCommand> forgetPasswordValidator,
        IValidator<ResetPasswordCommand> resetPasswordValidator,
        IValidator<RefreshTokenCommand> refreshTokenValidator) : IAuthenticationCommands
{
    public Task<Result> RegisterCommandHandler(RegisterCommand command, CancellationToken cancellationToken) =>
        ValidateAndExecute(
            command,
            createValidator,
            () => inner.RegisterCommandHandler(command, cancellationToken),
            cancellationToken);

    public Task<Result> ConfirmEmailCommandHandler(ConfirmEmailCommand command, CancellationToken cancellationToken) =>
        ValidateAndExecute(
                command,
                confirmEmailValidator,
                () => inner.ConfirmEmailCommandHandler(command, cancellationToken),
                cancellationToken);

    public Task<Result<AuthResponse>> LoginCommandHandler(LoginCommand command, CancellationToken cancellationToken) =>
        ValidateAndExecute(
            command,
            loginValidator,
            () => inner.LoginCommandHandler(command, cancellationToken),
            cancellationToken);

    public Task<Result> ResendConfirmationCodeCommandHandler(
        ResendEmailConfirmationCodeCommand command,
        CancellationToken cancellationToken) =>
            ValidateAndExecute(
                command,
                resendEmailConfirmationCodeValidator,
                () => inner.ResendConfirmationCodeCommandHandler(command, cancellationToken),
                cancellationToken);

    public Task<Result> ForgetPasswordCommandHandler(
        ForgetPasswordCommand command,
        CancellationToken cancellationToken) =>
            ValidateAndExecute(
                command,
                forgetPasswordValidator,
                () => inner.ForgetPasswordCommandHandler(command, cancellationToken),
                cancellationToken);

    public Task<Result> ResetPasswordCommandHandler(
        ResetPasswordCommand command,
        CancellationToken cancellationToken) =>
            ValidateAndExecute(
                command,
                resetPasswordValidator,
                () => inner.ResetPasswordCommandHandler(command, cancellationToken),
                cancellationToken);

    public Task<Result<AuthResponse>> RefreshTokenCommandHandler(
        RefreshTokenCommand command,
        CancellationToken cancellationToken) =>
            ValidateAndExecute(
                command,
                refreshTokenValidator,
                () => inner.RefreshTokenCommandHandler(command, cancellationToken),
                cancellationToken);

    private static async Task<Result> ValidateAndExecute<TCommand>(
        TCommand command,
        IValidator<TCommand> validator,
        Func<Task<Result>> execute,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(command, cancellationToken);

        if(!validationResult.IsValid)
        {
            var errorDictionary = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result.ValidationError<TCommand>(errorDictionary);
        }

        return await execute();
    }

    private static async Task<Result<TValue>> ValidateAndExecute<TCommand, TValue>(
        TCommand command,
        IValidator<TCommand> validator,
        Func<Task<Result<TValue>>> execute,
        CancellationToken cancellationToken)
    {
        ValidationResult validationResult = await validator.ValidateAsync(command, cancellationToken);

        if(!validationResult.IsValid)
        {
            var errorDictionary = validationResult.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

            return Result<TValue>.ValidationError<TCommand>(errorDictionary);
        }

        return await execute();
    }
}
