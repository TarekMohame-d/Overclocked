using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Domain.Common.Errors;

public static class AuthenticationErrors
{
    public static readonly Error InvalidConfirmationCodeCredentials = new(
        "Authentication.InvalidConfirmationCodeCredentials",
        "Email or Code is incorrect.",
        ErrorType.BadRequest);

    public static readonly Error EmailAlreadyConfirmed = new(
        "Authentication.EmailAlreadyConfirmed",
        "Email already confirmed, please login.",
        ErrorType.Conflict);

    public static readonly Error EmailConfirmationCodeExpired = new(
        "Authentication.EmailConfirmationCodeExpired",
        "Email confirmation code expired, please request new one.",
        ErrorType.BadRequest);

    public static readonly Error InvalidCredentials = new(
        "Authentication.InvalidCredentials",
        "Email or password is incorrect.",
        ErrorType.BadRequest);

    public static readonly Error EmailNotConfirmed = new(
        "Authentication.EmailNotConfirmed",
        "Email not confirmed, please confirm email first.",
        ErrorType.BadRequest);

    public static readonly Error InvalidAccessToken = new(
        "Authentication.InvalidAccessToken",
        "Invalid access token.",
        ErrorType.BadRequest);

    public static readonly Error InvalidRefreshToken = new(
        "Authentication.InvalidRefreshToken",
        "Invalid refresh token.",
        ErrorType.BadRequest);

    public static readonly Error InvalidResetPasswordCredentials = new(
        "Authentication.InvalidResetPasswordCredentials",
        "Email or Code is incorrect.",
        ErrorType.BadRequest);
}
