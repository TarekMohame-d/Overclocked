using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.DependencyInjection;
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

public class ValidatingAuthenticationCommandsDecorator(
    IAuthenticationCommands inner,
    IServiceProvider serviceProvider) : IAuthenticationCommands
{
    public Task<Result> RegisterCommandHandler(RegisterCommand command, CancellationToken cancellationToken) =>
        ValidateAndExecute(
            command,
            () => inner.RegisterCommandHandler(command, cancellationToken),
            cancellationToken);

    public Task<Result> ConfirmEmailCommandHandler(ConfirmEmailCommand command, CancellationToken cancellationToken) =>
        ValidateAndExecute(
                command,
                () => inner.ConfirmEmailCommandHandler(command, cancellationToken),
                cancellationToken);

    public Task<Result<AuthResponse>> LoginCommandHandler(LoginCommand command, CancellationToken cancellationToken) =>
        ValidateAndExecute(
            command,
            () => inner.LoginCommandHandler(command, cancellationToken),
            cancellationToken);

    public Task<Result> ResendConfirmationCodeCommandHandler(
        ResendEmailConfirmationCodeCommand command,
        CancellationToken cancellationToken) =>
            ValidateAndExecute(
                command,
                () => inner.ResendConfirmationCodeCommandHandler(command, cancellationToken),
                cancellationToken);

    public Task<Result> ForgetPasswordCommandHandler(
        ForgetPasswordCommand command,
        CancellationToken cancellationToken) =>
            ValidateAndExecute(
                command,
                () => inner.ForgetPasswordCommandHandler(command, cancellationToken),
                cancellationToken);

    public Task<Result> ResetPasswordCommandHandler(
        ResetPasswordCommand command,
        CancellationToken cancellationToken) =>
            ValidateAndExecute(
                command,
                () => inner.ResetPasswordCommandHandler(command, cancellationToken),
                cancellationToken);

    public Task<Result<AuthResponse>> RefreshTokenCommandHandler(
        RefreshTokenCommand command,
        CancellationToken cancellationToken) =>
            ValidateAndExecute(
                command,
                () => inner.RefreshTokenCommandHandler(command, cancellationToken),
                cancellationToken);

    private async Task<Result> ValidateAndExecute<TCommand>(
        TCommand command,
        Func<Task<Result>> execute,
        CancellationToken cancellationToken)
    {
        IValidator<TCommand> validator = serviceProvider.GetService<IValidator<TCommand>>()!;

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

    private async Task<Result<TValue>> ValidateAndExecute<TCommand, TValue>(
        TCommand command,
        Func<Task<Result<TValue>>> execute,
        CancellationToken cancellationToken)
    {
        IValidator<TCommand> validator = serviceProvider.GetService<IValidator<TCommand>>()!;

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
