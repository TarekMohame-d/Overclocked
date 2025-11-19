namespace Unit.Tests.Validations.Cart.TestCases;

public static class AddCartItemValidationTestCases
{
    public static IEnumerable<object[]> InvalidProductIdCases()
    {
        yield return [Guid.Empty];
    }

    public static IEnumerable<object[]> InvalidQuantityCases()
    {
        yield return [0];
        yield return [-1];
    }
}
