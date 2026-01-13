using FluentValidation.TestHelper;
using Overclocked.Application.Features.CategoryUseCases.CreateCategory;
using Overclocked.Unit.Tests.Validations.CategoryTests.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.CategoryTests;

public class CreateCategoryRequestValidatorTest
{
    [Theory]
    [MemberData(
        nameof(CreateCategoryValidationTestCases.InvalidNameCases),
        MemberType = typeof(CreateCategoryValidationTestCases)
    )]
    public async Task CreateCategoryRequestValidator_Should_ReturnError_When_NameIsInvalid(string? name)
    {
        // Arrange
        var validator = new CreateCategoryRequestValidator();
        var request = new CreateCategoryRequest { Name = name!, ImageUrl = "https://res.cloudinary.com/over-clocked/image.png" };

        // Act
        TestValidationResult<CreateCategoryRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(
        nameof(CreateCategoryValidationTestCases.InvalidImageUrlCases),
        MemberType = typeof(CreateCategoryValidationTestCases)
    )]
    public async Task CreateCategoryRequestValidator_Should_ReturnError_When_ImageIsInvalid(string? imageUrl)
    {
        // Arrange
        var validator = new CreateCategoryRequestValidator();
        var request = new CreateCategoryRequest { Name = "Category Name", ImageUrl = imageUrl! };

        // Act
        TestValidationResult<CreateCategoryRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl).Only();
    }

    [Fact]
    public async Task CreateCategoryRequestValidator_Should_ReturnError_When_AllFieldsAreInvalid()
    {
        // Arrange
        var validator = new CreateCategoryRequestValidator();
        var request = new CreateCategoryRequest { Name = string.Empty, ImageUrl = "imageUrl" };

        // Act
        TestValidationResult<CreateCategoryRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public async Task CreateCategoryRequestValidator_Should_Pass_When_AllFieldsAreValid()
    {
        // Arrange
        var validator = new CreateCategoryRequestValidator();
        var request = new CreateCategoryRequest
        {
            Name = "Category Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png",
        };

        // Act
        TestValidationResult<CreateCategoryRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count.ShouldBe(0);
    }
}
