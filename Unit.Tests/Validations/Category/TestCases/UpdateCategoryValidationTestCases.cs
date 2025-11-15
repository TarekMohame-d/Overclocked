namespace Unit.Tests.Validations.Category.TestCases;

public static class UpdateCategoryValidationTestCases
{
    public static IEnumerable<object[]> InvalidNameCases()
    {
        yield return [null!];
        yield return [""];
        yield return ["   "];
        yield return [new string('a', 55)]; // name too long
    }

    public static IEnumerable<object[]> InvalidImageUrlCases()
    {
        yield return [null!];
        yield return ["https://res.cloudinary.com/over-clocked.txt"]; // invalid extension
        yield return ["not-a-url"]; // invalid format
        yield return ["ftp://example.com/image.jpg"]; // invalid scheme
        yield return ["www.example.com/image.jpg"]; // missing scheme
        yield return ["https://www.example.com/image.jpg"]; // not same host
    }
}
