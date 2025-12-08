using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Domain.Common.Errors;

public static class AuthenticationErrors
{
    public static readonly Error InvalidConfirmationCodeCredentials = new(
        nameof(InvalidConfirmationCodeCredentials),
        ErrorType.BadRequest,
        "Email or Code is incorrect.");

    public static readonly Error EmailAlreadyConfirmed = new(
        nameof(EmailAlreadyConfirmed),
        ErrorType.Conflict,
        "Email already confirmed, please login.");

    public static readonly Error EmailConfirmationCodeExpired = new(
        nameof(EmailConfirmationCodeExpired),
        ErrorType.BadRequest,
        "Email confirmation code expired, please request new one.");

    public static readonly Error InvalidCredentials = new(
        nameof(InvalidCredentials),
        ErrorType.BadRequest,
        "Email or password is incorrect.");

    public static readonly Error EmailNotConfirmed = new(
        nameof(EmailNotConfirmed),
        ErrorType.BadRequest,
        "Email not confirmed, please confirm email first.");

    public static readonly Error InvalidAccessToken = new(
        nameof(InvalidAccessToken),
        ErrorType.BadRequest,
        "Invalid access token.");

    public static readonly Error InvalidRefreshToken = new(
        nameof(InvalidRefreshToken),
        ErrorType.BadRequest,
        "Invalid refresh token.");

    public static readonly Error InvalidResetPasswordCredentials = new(
        nameof(InvalidResetPasswordCredentials),
        ErrorType.BadRequest,
        "Email or Code is incorrect.");
}
