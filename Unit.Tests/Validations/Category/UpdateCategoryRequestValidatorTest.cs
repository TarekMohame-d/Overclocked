using Application.Services.Category.DTOs.Request;
using Application.Services.Category.Validations;
using Shouldly;
using Unit.Tests.Validations.Category.TestCases;

namespace Unit.Tests.Validations.Category;

public class UpdateCategoryRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateCategoryValidationTestCases.InvalidNameCases), MemberType = typeof(UpdateCategoryValidationTestCases))]
    public void UpdateCategoryRequestValidator_Should_ReturnError_When_NameValidationFails(string? name)
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.CreateVersion7(),
            Name = name!,
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        var validator = new UpdateCategoryRequestValidator();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Name").ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(UpdateCategoryValidationTestCases.InvalidImageUrlCases), MemberType = typeof(UpdateCategoryValidationTestCases))]
    public void UpdateCategoryRequestValidator_Should_ReturnError_When_ImageUrlValidationFails(string? imageUrl)
    {
        // Arrange
        var request = new UpdateCategoryRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageUrl = imageUrl!
        };

        var validator = new UpdateCategoryRequestValidator();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "ImageUrl").ShouldBeTrue();
    }
}
