using FluentValidation.TestHelper;
using Overclocked.Application.Features.BrandUseCases.CreateBrand;
using Overclocked.Unit.Tests.Validations.BrandTests.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.BrandTests;

public class CreateBrandRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(CreateBrandValidationTestCases.InvalidNameCases), MemberType = typeof(CreateBrandValidationTestCases))]
    public async Task CreateBrandRequestValidator_Should_ReturnError_When_NameIsInvalid(string? name)
    {
        // Arrange
        var validator = new CreateBrandRequestValidator();
        var request = new CreateBrandRequest { Name = name!, ImageUrl = "https://res.cloudinary.com/over-clocked/image.png" };

        // Act
        TestValidationResult<CreateBrandRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(nameof(CreateBrandValidationTestCases.InvalidImageUrlCases), MemberType = typeof(CreateBrandValidationTestCases))]
    public async Task CreateBrandRequestValidator_Should_ReturnError_When_ImageIsInvalid(string? imageUrl)
    {
        // Arrange
        var validator = new CreateBrandRequestValidator();
        var request = new CreateBrandRequest { Name = "Brand Name", ImageUrl = imageUrl! };

        // Act
        TestValidationResult<CreateBrandRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(1);
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl).Only();
    }

    [Fact]
    public async Task CreateBrandRequestValidator_Should_ReturnError_When_AllFieldsAreInvalid()
    {
        // Arrange
        var validator = new CreateBrandRequestValidator();
        var request = new CreateBrandRequest { Name = string.Empty, ImageUrl = "imageUrl" };

        // Act
        TestValidationResult<CreateBrandRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public async Task CreateBrandRequestValidator_Should_Pass_When_AllFieldsAreValid()
    {
        // Arrange
        var validator = new CreateBrandRequestValidator();
        var request = new CreateBrandRequest
        {
            Name = "Brand Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png",
        };

        // Act
        TestValidationResult<CreateBrandRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count.ShouldBe(0);
    }
}
