using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Unit.Tests.Validations.Category.TestCases;

public static class UpdateCategoryValidationTestCases
{
    public static IEnumerable<object[]> InvalidNameCases()
    {
        yield return new object[] { null! };
        yield return new object[] { "" };
        yield return new object[] { "   " };
        yield return new object[] { new string('a', 55) }; // name too long
    }

    public static IEnumerable<object[]> InvalidIdCases()
    {
        yield return new object[] { Guid.Empty };
    }

    public static IEnumerable<object[]> InvalidImageUrlCases()
    {
        yield return new object[] { "not-a-url" };                            // invalid format
        yield return new object[] { "ftp://example.com/image.jpg" };          // invalid scheme
        yield return new object[] { "www.example.com/image.jpg" };            // missing scheme
        yield return new object[] { "https://www.example.com/image.jpg" };    // not same host
    }

    public static IEnumerable<object[]> InvalidImageFileCases()
    {
        // Mock an empty file (Length = 0)
        var emptyFile = Substitute.For<IFormFile>();
        emptyFile.Length.Returns(0);
        emptyFile.FileName.Returns("image.jpg");

        // Mock a file with invalid extension
        var invalidExtFile = Substitute.For<IFormFile>();
        invalidExtFile.Length.Returns(500 * 1024); // 500 KB
        invalidExtFile.FileName.Returns("image.txt");

        // Mock an oversized file (> 2MB)
        var oversizedFile = Substitute.For<IFormFile>();
        oversizedFile.Length.Returns(3 * 1024 * 1024); // 3 MB
        oversizedFile.FileName.Returns("image.jpg");

        // Mock a file with both bad extension and size (for coverage)
        var badAllFile = Substitute.For<IFormFile>();
        badAllFile.Length.Returns(5 * 1024 * 1024); // 5 MB
        badAllFile.FileName.Returns("image.exe");

        yield return new object[] { emptyFile };
        yield return new object[] { invalidExtFile };
        yield return new object[] { oversizedFile };
        yield return new object[] { badAllFile };
    }

    public static IEnumerable<object[]> BothImageUrlAndImageCases()
    {
        var imageFile = Substitute.For<IFormFile>();
        imageFile.Length.Returns(1 * 1024 * 1024);
        imageFile.FileName.Returns("image.jpg");

        yield return new object[] { "https://example.com/image.jpg", imageFile }; // both not null
        yield return new object[] { null!, null! }; // both null
    }
}
