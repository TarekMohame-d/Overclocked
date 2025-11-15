namespace Unit.Tests.Validations.Tag.TestCases;

public class CreateTagValidationTestCases
{
    public static IEnumerable<object[]> InvalidNameCases()
    {
        yield return [null!];
        yield return [""];
        yield return ["   "];
        yield return [new string('a', 55)];
    }
}
