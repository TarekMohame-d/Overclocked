using Application.Features.Category.Commands.UpdateCategory;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;
using Unit.Tests.Validations.Category.TestCases;

namespace Unit.Tests.Validations.Category;

public class UpdateCategoryCommandValidatorTest
{
    private static IFormFile CreateImageFile(string fileName = "test.jpg", long size = 1024, string contentType = "image/jpeg")
    {
        var file = Substitute.For<IFormFile>();
        file.FileName.Returns(fileName);
        file.Length.Returns(size);
        file.ContentType.Returns(contentType);
        file.OpenReadStream().Returns(new MemoryStream(new byte[size]));
        return file;
    }

    [Theory]
    [MemberData(nameof(UpdateCategoryValidationTestCases.InvalidIdCases), MemberType = typeof(UpdateCategoryValidationTestCases))]
    public void Handle_WhenIdValidationFails_ShouldReturnError(Guid? id)
    {
        // Arrange
        var command = new UpdateCategoryWithIdCommand
        {
            Id = (Guid)id!,
            Name = "Nike",
            ImageFile = CreateImageFile()
        };

        var validator = new UpdateCategoryCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Id").ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(UpdateCategoryValidationTestCases.InvalidNameCases), MemberType = typeof(UpdateCategoryValidationTestCases))]
    public void Handle_WhenNameValidationFails_ShouldReturnError(string? name)
    {
        // Arrange
        var command = new UpdateCategoryWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = name!,
            ImageFile = CreateImageFile()
        };

        var validator = new UpdateCategoryCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Name").ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(UpdateCategoryValidationTestCases.BothImageUrlAndImageCases), MemberType = typeof(UpdateCategoryValidationTestCases))]
    public void Handle_WhenBothImageUrlAndImageValidationFails_ShouldReturnError(string? imageUrl, IFormFile? imageFile)
    {
        // Arrange
        var command = new UpdateCategoryWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageUrl = imageUrl,
            ImageFile = imageFile
        };

        var validator = new UpdateCategoryCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "ImageFile" || e.PropertyName == "ImageUrl").ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(UpdateCategoryValidationTestCases.InvalidImageUrlCases), MemberType = typeof(UpdateCategoryValidationTestCases))]
    public void Handle_WhenImageUrlValidationFails_ShouldReturnError(string? imageUrl)
    {
        // Arrange
        var command = new UpdateCategoryWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageUrl = imageUrl
        };

        var validator = new UpdateCategoryCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "ImageUrl").ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(UpdateCategoryValidationTestCases.InvalidImageFileCases), MemberType = typeof(UpdateCategoryValidationTestCases))]
    public void Handle_WhenImageFileValidationFails_ShouldReturnError(IFormFile? imageFile)
    {
        // Arrange
        var command = new UpdateCategoryWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageFile = imageFile!
        };

        var validator = new UpdateCategoryCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "ImageFile").ShouldBeTrue();
    }
}
