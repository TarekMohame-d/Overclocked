using Application.Services.Brand.DTOs.Request;
using Application.Services.Brand.Validations;
using Shouldly;
using Unit.Tests.Validations.Brand.TestCases;

namespace Unit.Tests.Validations.Brand;

public class UpdateBrandRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateBrandValidationTestCases.InvalidNameCases), MemberType = typeof(UpdateBrandValidationTestCases))]
    public void UpdateBrandRequestValidator_Should_ReturnError_When_NameValidationFails(string? name)
    {
        // Arrange
        var request = new UpdateBrandRequest
        {
            Id = Guid.CreateVersion7(),
            Name = name!,
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        var validator = new UpdateBrandRequestValidator();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Name").ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(UpdateBrandValidationTestCases.InvalidImageUrlCases), MemberType = typeof(UpdateBrandValidationTestCases))]
    public void UpdateBrandRequestValidator_Should_ReturnError_When_ImageUrlValidationFails(string? imageUrl)
    {
        // Arrange
        var request = new UpdateBrandRequest
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageUrl = imageUrl!
        };

        var validator = new UpdateBrandRequestValidator();

        // Act
        var result = validator.Validate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "ImageUrl").ShouldBeTrue();
    }
}
