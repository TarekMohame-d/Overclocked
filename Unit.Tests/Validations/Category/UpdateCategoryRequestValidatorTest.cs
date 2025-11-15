using Application.Services.Category.DTOs.Request;
using Application.Services.Category.Validations;
using FluentValidation.TestHelper;
using Shouldly;
using Unit.Tests.Validations.Category.TestCases;

namespace Unit.Tests.Validations.Category;

public class UpdateCategoryRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateCategoryValidationTestCases.InvalidNameCases),
        MemberType = typeof(UpdateCategoryValidationTestCases))]
    public void UpdateCategoryRequestValidator_Should_ReturnError_When_NameValidationFails(string? name)
    {
        // Arrange
        var validator = new UpdateCategoryRequestValidator();

        var request = new UpdateCategoryRequest
        {
            Id = Guid.CreateVersion7(),
            Name = name!,
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        // Act
        TestValidationResult<UpdateCategoryRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(nameof(UpdateCategoryValidationTestCases.InvalidImageUrlCases),
        MemberType = typeof(UpdateCategoryValidationTestCases))]
    public void UpdateCategoryRequestValidator_Should_ReturnError_When_ImageUrlValidationFails(string? imageUrl)
    {
        // Arrange
        var validator = new UpdateCategoryRequestValidator();

        var request = new UpdateCategoryRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Category Name",
            ImageUrl = imageUrl!
        };

        // Act
        TestValidationResult<UpdateCategoryRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl).Only();
    }
}
