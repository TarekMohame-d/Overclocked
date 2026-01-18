using FluentValidation.TestHelper;
using Overclocked.Application.Features.BrandUseCases.UpdateBrand;
using Overclocked.Unit.Tests.Validations.BrandTests.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.BrandTests;

public class UpdateBrandRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateBrandValidationTestCases.InvalidNameCases), MemberType = typeof(UpdateBrandValidationTestCases))]
    public void UpdateBrandRequestValidator_Should_ReturnError_When_NameValidationFails(string? name)
    {
        // Arrange
        var validator = new UpdateBrandRequestValidator();

        var request = new UpdateBrandRequest
        {
            Id = Guid.NewGuid(),
            Name = name!,
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png",
        };

        // Act
        TestValidationResult<UpdateBrandRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(nameof(UpdateBrandValidationTestCases.InvalidImageUrlCases), MemberType = typeof(UpdateBrandValidationTestCases))]
    public void UpdateBrandRequestValidator_Should_ReturnError_When_ImageUrlValidationFails(string? imageUrl)
    {
        // Arrange
        var validator = new UpdateBrandRequestValidator();

        var request = new UpdateBrandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Brand Name",
            ImageUrl = imageUrl!,
        };

        // Act
        TestValidationResult<UpdateBrandRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl).Only();
    }

    [Fact]
    public async Task UpdateBrandRequestValidator_Should_ReturnError_When_AllFieldsAreInvalid()
    {
        // Arrange
        var validator = new UpdateBrandRequestValidator();

        var request = new UpdateBrandRequest
        {
            Id = Guid.NewGuid(),
            Name = string.Empty,
            ImageUrl = "imageUrl",
        };

        // Act
        TestValidationResult<UpdateBrandRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public async Task UpdateBrandRequestValidator_Should_Pass_When_AllFieldsAreValid()
    {
        // Arrange
        var validator = new UpdateBrandRequestValidator();

        var request = new UpdateBrandRequest
        {
            Id = Guid.NewGuid(),
            Name = "Brand Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png",
        };

        // Act
        TestValidationResult<UpdateBrandRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count.ShouldBe(0);
    }
}
