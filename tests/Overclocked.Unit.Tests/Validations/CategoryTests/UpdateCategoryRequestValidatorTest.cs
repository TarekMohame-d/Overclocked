using FluentValidation.TestHelper;
using Overclocked.Application.Features.CategoryUseCases.UpdateCategory;
using Overclocked.Unit.Tests.Validations.CategoryTests.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.CategoryTests;

public class UpdateCategoryRequestValidatorTest
{
    [Theory]
    [MemberData(
        nameof(UpdateCategoryValidationTestCases.InvalidNameCases),
        MemberType = typeof(UpdateCategoryValidationTestCases)
    )]
    public void UpdateCategoryRequestValidator_Should_ReturnError_When_NameValidationFails(string? name)
    {
        // Arrange
        var validator = new UpdateCategoryRequestValidator();
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Id = categoryId,
            Name = name!,
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png",
        };

        // Act
        TestValidationResult<UpdateCategoryRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateCategoryValidationTestCases.InvalidImageUrlCases),
        MemberType = typeof(UpdateCategoryValidationTestCases)
    )]
    public void UpdateCategoryRequestValidator_Should_ReturnError_When_ImageUrlValidationFails(string? imageUrl)
    {
        // Arrange
        var validator = new UpdateCategoryRequestValidator();
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Id = categoryId,
            Name = "Category Name",
            ImageUrl = imageUrl!,
        };

        // Act
        TestValidationResult<UpdateCategoryRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl).Only();
    }

    [Fact]
    public async Task UpdateCategoryRequestValidator_Should_ReturnError_When_AllFieldsAreInvalid()
    {
        // Arrange
        var validator = new UpdateCategoryRequestValidator();
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Id = categoryId,
            Name = string.Empty,
            ImageUrl = "imageUrl",
        };

        // Act
        TestValidationResult<UpdateCategoryRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public async Task UpdateCategoryRequestValidator_Should_Pass_When_AllFieldsAreValid()
    {
        // Arrange
        var validator = new UpdateCategoryRequestValidator();
        var categoryId = Guid.NewGuid();
        var request = new UpdateCategoryRequest
        {
            Id = categoryId,
            Name = "Category Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png",
        };

        // Act
        TestValidationResult<UpdateCategoryRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count.ShouldBe(0);
    }
}
