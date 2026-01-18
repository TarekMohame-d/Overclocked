using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;

namespace Overclocked.Unit.Tests.Validations.ProductTests.TestCases;

public static class CreateProductValidationTestCases
{
    public static IEnumerable<object[]> InvalidNameCases()
    {
        yield return [null!];
        yield return [string.Empty];
        yield return ["   "];
        yield return [new string('a', 55)]; // exceeds 50 chars
    }

    public static IEnumerable<object[]> InvalidThumbnailCases()
    {
        yield return [null!];
        yield return [string.Empty];
        yield return ["   "];
        yield return ["not-a-url"];
        yield return ["ftp://example.com/image.jpg"];
        yield return ["www.example.com/image.jpg"];
        yield return ["https://www.example.com/image.jpg"]; // wrong host
        yield return ["https://res.cloudinary.com/over-clocked/image.txt"]; // invalid URL path
        yield return ["ftp://res.cloudinary.com/over-clocked/image.png"]; // invalid URL path
    }

    public static IEnumerable<object[]> InvalidDescriptionCases()
    {
        yield return [null!];
        yield return [string.Empty];
        yield return ["   "];
        yield return [new string('a', 505)]; // exceeds 500 chars
    }

    public static IEnumerable<object[]> InvalidPriceCases()
    {
        yield return [-1.0m];
        yield return [0.0m];
    }

    public static IEnumerable<object[]> InvalidStockCases()
    {
        yield return [-1];
    }

    public static IEnumerable<object[]> InvalidDiscountCases()
    {
        yield return [-0.1m];
        yield return [1.0m];
        yield return [1.5m];
    }

    public static IEnumerable<object[]> InvalidImagesCases()
    {
        yield return [new[] { "https://res.cloudinary.com/over-clocked/image.txt" }];
        yield return [new[] { "ftp://res.cloudinary.com/over-clocked/image.png" }];
        yield return [new[] { "not-a-url" }];
        yield return [new[] { "ftp://example.com/image.jpg" }];
        yield return [new[] { "www.example.com/image.jpg" }];
        yield return [new[] { "https://www.example.com/image.jpg" }];
    }

    public static IEnumerable<object[]> InvalidTagsCases()
    {
        yield return [null!]; // no tags
        yield return [new List<Guid>()]; // empty list
    }

    public static IEnumerable<object[]> InvalidSpecificationsCases()
    {
        // null
        yield return [null!];

        // Empty or invalid
        yield return
        [
            new List<ProductSpecificationDto>
            {
                new ProductSpecificationDto { Name = null!, Value = null! },
                new ProductSpecificationDto { Name = string.Empty, Value = string.Empty },
            },
        ];

        // Too long name and value
        yield return
        [
            new List<ProductSpecificationDto>
            {
                new ProductSpecificationDto { Name = new string('N', 55), Value = new string('V', 350) },
            },
        ];

        // Duplicate names
        yield return
        [
            new List<ProductSpecificationDto>
            {
                new ProductSpecificationDto { Name = "Name", Value = "Value" },
                new ProductSpecificationDto { Name = "Name", Value = "Another Value" },
            },
        ];

        // Empty list
        yield return [new List<ProductSpecificationDto>()];
    }
}
