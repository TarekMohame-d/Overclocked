namespace Unit.Tests.Validations.Authentication.TestCases;

public class RefreshTokenValidationTestCases
{
    public static IEnumerable<object[]> InvalidAccessTokenCases()
    {
        yield return [null!];
        yield return [""];
        yield return ["   "];
        yield return ["wrong-jwt-formate"];
    }

    public static IEnumerable<object[]> InvalidRefreshTokenCases()
    {
        yield return [null!];
        yield return [""];
        yield return ["   "];
    }
}
