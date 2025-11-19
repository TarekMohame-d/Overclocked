namespace Unit.Tests.Validations.Authentication.TestCases;

public class ConfirmEmailValidationTestCases
{
    public static IEnumerable<object[]> InvalidEmailCases()
    {
        yield return [null!];
        yield return [""];
        yield return ["   "];
        yield return [new string('a', 105)];
        yield return ["wrong-email-formate"];
    }

    public static IEnumerable<object[]> InvalidCodeCases()
    {
        yield return [null!];
        yield return [""];
        yield return ["   "];
        yield return [new string('a', 7)];
        yield return [new string('a', 5)];
    }
}
