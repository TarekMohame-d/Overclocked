using System.Text.RegularExpressions;

namespace Overclocked.Domain.UserAggregate;

internal static class UserValidator
{
    public static Dictionary<string, string[]> Validate(string firstName, string lastName, string email, string phone)
    {
        var validationResults = new List<Dictionary<string, string[]>>
        {
            ValidateFirstName(firstName),
            ValidateLastName(lastName),
            ValidateEmail(email),
            ValidatePhone(phone),
        };

        return validationResults
            .SelectMany(dict => dict)
            .GroupBy(kvp => kvp.Key)
            .ToDictionary(group => group.Key, group => group.SelectMany(kvp => kvp.Value).ToArray());
    }

    private static Dictionary<string, string[]> ValidateFirstName(string firstName)
    {
        var validationKey = "FirstName";
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(firstName))
            errors.Add("First name is required");

        if (firstName?.Length > 20)
            errors.Add("First name is too long");

        return errors.Count > 0 ? new Dictionary<string, string[]> { { validationKey, errors.ToArray() } } : [];
    }

    private static Dictionary<string, string[]> ValidateLastName(string lastName)
    {
        var validationKey = "LastName";
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(lastName))
            errors.Add("Last name is required");

        if (lastName?.Length > 20)
            errors.Add("Last name is too long");

        return errors.Count > 0 ? new Dictionary<string, string[]> { { validationKey, errors.ToArray() } } : [];
    }

    private static Dictionary<string, string[]> ValidateEmail(string email)
    {
        var validationKey = "Email";
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(email))
            errors.Add("Email is required");

        var regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");

        if (!(regex?.IsMatch(email) ?? false))
            errors.Add("Email is not valid email address");

        if (email?.Length > 100)
            errors.Add("Email is too long");

        return errors.Count > 0 ? new Dictionary<string, string[]> { { validationKey, errors.ToArray() } } : [];
    }

    private static Dictionary<string, string[]> ValidatePhone(string phone)
    {
        var validationKey = "Phone";
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(phone))
            errors.Add("Phone is required");

        var regex = new Regex(@"^\+?\d{10,15}$");

        if (!(regex?.IsMatch(phone) ?? false))
            errors.Add("Phone is not valid phone number");

        if (phone?.Length > 20)
            errors.Add("Phone is too long");

        return errors.Count > 0 ? new Dictionary<string, string[]> { { validationKey, errors.ToArray() } } : [];
    }
}
