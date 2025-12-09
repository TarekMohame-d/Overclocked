namespace Overclocked.Unit.Tests.Validations.Brand.TestCases;

public static class CreateBrandValidationTestCases
{
    public static IEnumerable<object[]> InvalidNameCases()
    {
        yield return [null!];
        yield return [string.Empty];
        yield return ["   "];
        yield return [new string('a', 55)];
    }

    public static IEnumerable<object[]> InvalidImageUrlCases()
    {
        yield return [null!];
        yield return ["https://res.cloudinary.com/over-clocked.txt"]; // invalid extension
        yield return ["not-a-url"]; // invalid format
        yield return ["ftp://res.cloudinary.com/over-clocked.jpg"]; // invalid scheme
        yield return ["www.example.com/image.jpg"]; // missing scheme
        yield return ["https://www.example.com/image.jpg"]; // not same host
    }
}
