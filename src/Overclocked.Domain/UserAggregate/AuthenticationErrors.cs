using Overclocked.SharedKernel;

namespace Overclocked.Domain.UserAggregate;

public static class AuthenticationErrors
{
    public static Error InvalidConfirmationCodeCredentials =>
        Error.BadRequest("Authentication.InvalidConfirmationCodeCredentials", "Email or Code is incorrect.");

    public static Error EmailAlreadyConfirmed =>
        Error.BadRequest("Authentication.EmailAlreadyConfirmed", "Email already confirmed, please login.");

    public static Error EmailConfirmationCodeExpired =>
        Error.BadRequest(
            "Authentication.EmailConfirmationCodeExpired",
            "Email confirmation code expired, please request new one."
        );

    public static Error InvalidCredentials =>
        Error.BadRequest("Authentication.InvalidCredentials", "Email or password is incorrect.");

    public static Error UserIsInactive => new("Authentication.UserIsInactive", "User is inactive.", ErrorType.Forbidden);

    public static Error EmailNotConfirmed =>
        Error.BadRequest("Authentication.EmailNotConfirmed", "Email not confirmed, please confirm email first.");

    public static Error InvalidAccessToken => Error.BadRequest("Authentication.InvalidAccessToken", "Invalid access token.");

    public static Error InvalidRefreshToken => Error.BadRequest("Authentication.InvalidRefreshToken", "Invalid refresh token.");

    public static Error InvalidResetPasswordCredentials =>
        Error.BadRequest("Authentication.InvalidResetPasswordCredentials", "Email or Code is incorrect.");

    public static Error InvalidPassword =>
        Error.Validation(
            "Authentication.InvalidPassword",
            "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character."
        );

    public static Error RefreshTokenIsRequired => Error.Validation("Authentication.RefreshToken", "Refresh token is required.");

    public static Error InvalidDeviceId => Error.Validation("Authentication.DeviceId", "Invalid device id.");

    public static Error PhoneAlreadyExists => Error.Validation("Authentication.Phone", "Phone number already exists.");

    public static Error EmailAlreadyExists => Error.Validation("Authentication.Email", "Email already exists.");
}
