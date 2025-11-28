namespace Unit.Tests.Validations.ReviewReply.TestCases;

public static class CreateReviewReplyValidationTestCases
{
    public static IEnumerable<object[]> InvalidReplyCases()
    {
        yield return [null!];
        yield return [""];
        yield return ["   "];
        yield return [new string('a', 501)];
    }
}
