namespace Unit.Tests.Validations.Authentication.TestCases;

public class ResendEmailConfirmationCodeValidationTestCases
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
}
