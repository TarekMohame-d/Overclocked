namespace Overclocked.Unit.Tests.Validations.Authentication.TestCases;

public static class RefreshTokenValidationTestCases
{
    public static IEnumerable<object[]> InvalidAccessTokenCases()
    {
        yield return [null!];
        yield return [string.Empty];
        yield return ["   "];
        yield return ["wrong-jwt-formate"];
    }

    public static IEnumerable<object[]> InvalidRefreshTokenCases()
    {
        yield return [null!];
        yield return [string.Empty];
        yield return ["   "];
    }
}
