using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Unit.Tests.Validations.Category.TestCases;

public static class CreateCategoryValidationTestCases
{
    public static IEnumerable<object[]> InvalidNameCases()
    {
        yield return new object[] { null! };
        yield return new object[] { "" };
        yield return new object[] { "   " };
        yield return new object[] { new string('a', 55) };
    }

    public static IEnumerable<object[]> InvalidImageUrlCases()
    {
        yield return new object[] { null! };
        yield return new object[] { "https://res.cloudinary.com/over-clocked.txt" };    // invalid extension
        yield return new object[] { "not-a-url" };                                      // invalid format
        yield return new object[] { "ftp://example.com/image.jpg" };                    // invalid scheme
        yield return new object[] { "www.example.com/image.jpg" };                      // missing scheme
        yield return new object[] { "https://www.example.com/image.jpg" };              // not same host
    }
}
