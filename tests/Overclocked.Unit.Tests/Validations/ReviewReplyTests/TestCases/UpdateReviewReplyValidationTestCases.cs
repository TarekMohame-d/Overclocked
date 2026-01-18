namespace Overclocked.Unit.Tests.Validations.ReviewReplyTests.TestCases;

public static class UpdateReviewReplyValidationTestCases
{
    public static IEnumerable<object[]> InvalidReplyCases()
    {
        yield return [null!];
        yield return [string.Empty];
        yield return ["   "];
        yield return [new string('a', 501)];
    }
}
