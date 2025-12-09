namespace Overclocked.Unit.Tests.Validations.Tag.TestCases;

public static class CreateTagValidationTestCases
{
    public static IEnumerable<object[]> InvalidNameCases()
    {
        yield return [null!];
        yield return [string.Empty];
        yield return ["   "];
        yield return [new string('a', 55)];
    }
}
