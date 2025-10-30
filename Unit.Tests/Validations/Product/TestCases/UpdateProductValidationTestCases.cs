using Application.Services.Product.DTOs.Request;

namespace Unit.Tests.Validations.Product.TestCases;

public class UpdateProductValidationTestCases
{
    public static IEnumerable<object[]> InvalidNameCases()
    {
        yield return new object[] { null! };
        yield return new object[] { "" };
        yield return new object[] { "   " };
        yield return new object[] { new string('a', 55) }; // exceeds 50 chars
    }

    public static IEnumerable<object[]> InvalidThumbnailCases()
    {
        yield return new object[] { null! };
        yield return new object[] { "" };
        yield return new object[] { "   " };
        yield return new object[] { "not-a-url" };
        yield return new object[] { "ftp://example.com/image.jpg" };
        yield return new object[] { "www.example.com/image.jpg" };
        yield return new object[] { "https://www.example.com/image.jpg" }; // wrong host
        yield return new object[] { "https://res.cloudinary.com/over-clocked.txt" }; // invalid URL path
    }

    public static IEnumerable<object[]> InvalidDescriptionCases()
    {
        yield return new object[] { null! };
        yield return new object[] { "" };
        yield return new object[] { "   " };
        yield return new object[] { new string('a', 505) }; // exceeds 500 chars
    }

    public static IEnumerable<object[]> InvalidPriceCases()
    {
        yield return new object[] { -1.0m };
        yield return new object[] { 0.0m };
    }

    public static IEnumerable<object[]> InvalidStockCases()
    {
        yield return new object[] { -1 };
    }

    public static IEnumerable<object[]> InvalidDiscountCases()
    {
        yield return new object[] { -0.1m };
        yield return new object[] { 1.0m };
        yield return new object[] { 1.5m };
    }

    public static IEnumerable<object[]> InvalidImagesCases()
    {
        yield return new object[] { new[] { "https://res.cloudinary.com/over-clocked.txt" } };
        yield return new object[] { new[] { "not-a-url" } };
        yield return new object[] { new[] { "ftp://example.com/image.jpg" } };
        yield return new object[] { new[] { "www.example.com/image.jpg" } };
        yield return new object[] { new[] { "https://www.example.com/image.jpg" } };
    }

    public static IEnumerable<object[]> InvalidTagsCases()
    {
        yield return new object[] { null! }; // no tags
        yield return new object[] { new List<Guid>() }; // empty list
    }

    public static IEnumerable<object[]> InvalidSpecificationsCases()
    {
        // Empty or invalid
        yield return new object[]
        {
                new List<UpdateProductRequest.Specs>
                {
                    new() { Name = null!, Value = null! },
                }
        };

        // Too long name and value
        yield return new object[]
        {
                new List<UpdateProductRequest.Specs>
                {
                    new() { Name = new string('N', 55), Value = new string('V', 350) },
                }
        };

        // Duplicate names
        yield return new object[]
        {
                new List<UpdateProductRequest.Specs>
                {
                    new() { Name = "CPU", Value = "Intel i7" },
                    new() { Name = "CPU", Value = "Intel i9" }
                }
        };

        // Empty list
        yield return new object[] { new List<UpdateProductRequest.Specs>() };
    }
}
