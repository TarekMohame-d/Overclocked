namespace Overclocked.Unit.Tests.Validations.CartTests.TestCases;

public static class UpdateCartItemValidationTestCases
{
    public static IEnumerable<object[]> InvalidQuantityCases()
    {
        yield return [0];
        yield return [-1];
    }
}
