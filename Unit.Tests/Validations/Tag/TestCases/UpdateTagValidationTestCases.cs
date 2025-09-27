namespace Unit.Tests.Validations.Tag.TestCases;

public class UpdateTagValidationTestCases
{
    public static IEnumerable<object[]> InvalidNameCases()
    {
        yield return new object[] { null! };
        yield return new object[] { "" };
        yield return new object[] { "   " };
        yield return new object[] { new string('a', 55) };
    }

    public static IEnumerable<object[]> InvalidIdCases()
    {
        yield return new object[] { Guid.Empty };
    }
}
