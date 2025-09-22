using Microsoft.AspNetCore.Http;
using NSubstitute;

namespace Unit.Tests.Validations.Brand.TestCases;

public static class CreateBrandValidationTestCases
{
    public static IEnumerable<object[]> InvalidNameCases()
    {
        yield return new object[] { null! };
        yield return new object[] { "" };
        yield return new object[] { "   " };
        yield return new object[] { new string('a', 55) }; // long name
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

        yield return new object[] { null! };
        yield return new object[] { emptyFile };
        yield return new object[] { invalidExtFile };
        yield return new object[] { oversizedFile };
        yield return new object[] { badAllFile };
    }
}
