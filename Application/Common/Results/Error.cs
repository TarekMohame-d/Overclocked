namespace Application.Common.Results;

public sealed record Error(
    string Code,
    ErrorType Type,
    string Description,
    Dictionary<string, string[]>? ValidationErrors = null);

// Predefined errors (avoids magic strings)
public static class Errors
{
    public static readonly Error InvalidAccessToken =
        new(nameof(InvalidAccessToken), ErrorType.BadRequest, "Invalid access token.");

    public static readonly Error InvalidRefreshToken =
        new(nameof(InvalidRefreshToken), ErrorType.BadRequest, "Invalid refresh token.");

    public static readonly Error UserNotFound =
        new(nameof(UserNotFound), ErrorType.NotFound, "User with this email not exists.");

    public static readonly Error InvalidConfirmationCode =
        new(nameof(InvalidConfirmationCode), ErrorType.BadRequest, "Invalid confirmation code.");

    public static readonly Error EmailAlreadyConfirmed = new(nameof(EmailAlreadyConfirmed), ErrorType.Conflict,
        "Email already confirmed, please login.");

    public static readonly Error EmailConfirmationCodeExpired = new(nameof(EmailConfirmationCodeExpired),
        ErrorType.BadRequest, "Email confirmation code expired, please request new one.");

    public static readonly Error EmailNotConfirmed = new(nameof(EmailNotConfirmed), ErrorType.BadRequest,
        "Email not confirmed, please confirm email first.");

    public static readonly Error InvalidCredentials =
        new(nameof(InvalidCredentials), ErrorType.BadRequest, "Email or password is incorrect.");

    public static readonly Error InvalidResetPasswordCredentials =
        new(nameof(InvalidResetPasswordCredentials), ErrorType.BadRequest, "Email or Code is incorrect.");

    public static readonly Error InvalidConfirmationCodeCredentials =
        new(nameof(InvalidConfirmationCodeCredentials), ErrorType.BadRequest, "Email or Code is incorrect.");

    public static readonly Error FileStorageError =
        new(nameof(FileStorageError), ErrorType.FileStorageError, "Upload file failed.");

    public static readonly Error InternalServerError =
        new(nameof(InternalServerError), ErrorType.InternalServerError, "An error occurred.");

    // Brand errors
    public static readonly Error BrandNotFound = new(nameof(BrandNotFound), ErrorType.NotFound, "Brand not found.");

    public static readonly Error BrandNameAlreadyExists =
        new(nameof(BrandNameAlreadyExists), ErrorType.Conflict, "Brand name already exists.");

    // Category errors
    public static readonly Error CategoryNotFound =
        new(nameof(CategoryNotFound), ErrorType.NotFound, "Category not found.");

    public static readonly Error CategoryNameAlreadyExists = new(nameof(CategoryNameAlreadyExists), ErrorType.Conflict,
        "Category name already exists.");

    // Tag errors
    public static readonly Error TagNotFound = new(nameof(TagNotFound), ErrorType.NotFound, "Tag not found.");

    public static readonly Error TagNameAlreadyExists =
        new(nameof(TagNameAlreadyExists), ErrorType.Conflict, "Tag name already exists.");

    // Product errors
    public static readonly Error ProductNotFound =
        new(nameof(ProductNotFound), ErrorType.NotFound, "Product not found.");

    public static readonly Error ProductNameAlreadyExists =
        new(nameof(ProductNameAlreadyExists), ErrorType.Conflict, "Product name already exists.");
}
