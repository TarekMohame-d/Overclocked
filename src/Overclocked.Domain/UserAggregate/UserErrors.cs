using Overclocked.SharedKernel;

namespace Overclocked.Domain.UserAggregate;

public static class UserErrors
{
    public static ValidationError FirstNameIsRequired =>
        new(new Dictionary<string, string[]> { { "FirstName", ["FirstName is required."] } });

    public static ValidationError FirstNameIsTooLong =>
        new(new Dictionary<string, string[]> { { "FirstName", ["FirstName must be less than 20 characters."] } });

    public static ValidationError LastNameIsRequired =>
        new(new Dictionary<string, string[]> { { "LastName", ["LastName is required."] } });

    public static ValidationError LastNameIsTooLong =>
        new(new Dictionary<string, string[]> { { "LastName", ["LastName must be less than 20 characters."] } });

    public static ValidationError EmailIsRequired =>
        new(new Dictionary<string, string[]> { { "Email", ["Email is required."] } });

    public static ValidationError InvalidEmail =>
        new(new Dictionary<string, string[]> { { "Email", ["Email is not valid email address."] } });

    public static ValidationError PhoneIsRequired =>
        new(new Dictionary<string, string[]> { { "Phone", ["Phone is required."] } });

    public static ValidationError InvalidPhone =>
        new(new Dictionary<string, string[]> { { "Phone", ["Phone is not valid phone number."] } });

    public static Error NotFound(Guid id) => Error.NotFound("User.NotFound", $"The User with Id: '{id}' was not found.");
}
