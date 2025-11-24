namespace Unit.Tests.Validations.Authentication.TestCases;

public class RegisterValidationTestCases
{
    public static IEnumerable<object[]> InvalidEmailCases()
    {
        yield return [null!];
        yield return [""];
        yield return ["   "];
        yield return [new string('a', 105)];
        yield return ["wrong-email-formate"];
        yield return ["temp@temp"];
    }

    public static IEnumerable<object[]> InvalidPasswordCases()
    {
        yield return [null!];
        yield return [""];
        yield return ["   "];
        yield return [new string('a', 7)];
        yield return ["wrong-password-formate"];
        yield return ["Test12345"];
        yield return ["test12345"];
        yield return ["test12345@"];
    }

    public static IEnumerable<object[]> InvalidFirstNameCases()
    {
        yield return [null!];
        yield return [""];
        yield return ["   "];
        yield return [new string('a', 21)];
    }

    public static IEnumerable<object[]> InvalidLastNameCases()
    {
        yield return [null!];
        yield return [""];
        yield return ["   "];
        yield return [new string('a', 21)];
    }

    public static IEnumerable<object[]> InvalidPhoneNumberCases()
    {
        yield return [null!];
        yield return [""];
        yield return ["   "];
        yield return [new string('a', 21)];
        yield return ["013515abc125"];
    }
}
