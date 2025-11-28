using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.Validations;
using FluentValidation.TestHelper;
using Shouldly;
using Unit.Tests.Validations.Brand.TestCases;

namespace Unit.Tests.Validations.Brand;

public class UpdateBrandRequestValidatorTest
{
    [Theory]
    [MemberData(
        nameof(UpdateBrandValidationTestCases.InvalidNameCases),
        MemberType = typeof(UpdateBrandValidationTestCases)
    )]
    public void UpdateBrandRequestValidator_Should_ReturnError_When_NameValidationFails(string? name)
    {
        // Arrange
        var validator = new UpdateBrandRequestValidator();

        var request = new UpdateBrandRequestBody
        {
            Name = name!,
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png",
        };

        // Act
        TestValidationResult<UpdateBrandRequestBody> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateBrandValidationTestCases.InvalidImageUrlCases),
        MemberType = typeof(UpdateBrandValidationTestCases)
    )]
    public void UpdateBrandRequestValidator_Should_ReturnError_When_ImageUrlValidationFails(string? imageUrl)
    {
        // Arrange
        var validator = new UpdateBrandRequestValidator();

        var request = new UpdateBrandRequestBody
        {
            Name = "Brand Name",
            ImageUrl = imageUrl!,
        };

        // Act
        TestValidationResult<UpdateBrandRequestBody> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl).Only();
    }
}
