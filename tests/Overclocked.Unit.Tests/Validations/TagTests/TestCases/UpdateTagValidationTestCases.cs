namespace Overclocked.Unit.Tests.Validations.TagTests.TestCases;

public static class UpdateTagValidationTestCases
{
    public static IEnumerable<object[]> InvalidNameCases()
    {
        yield return [null!];
        yield return [string.Empty];
        yield return ["   "];
        yield return [new string('a', 55)];
    }
}
