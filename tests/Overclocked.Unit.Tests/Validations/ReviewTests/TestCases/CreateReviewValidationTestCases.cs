namespace Overclocked.Unit.Tests.Validations.ReviewTests.TestCases;

public static class CreateReviewValidationTestCases
{
    public static IEnumerable<object[]> InvalidRatingCases()
    {
        yield return [0];
        yield return [-1];
        yield return [6];
    }

    public static IEnumerable<object[]> InvalidCommentCases()
    {
        yield return [null!];
        yield return [string.Empty];
        yield return ["   "];
        yield return [new string('a', 501)];
    }
}
