using FluentValidation.TestHelper;
using Overclocked.Application.Brand.Commands.UpdateBrand;
using Overclocked.Domain.BrandAggregate.ValueObjects;
using Overclocked.Unit.Tests.Validations.Brand.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Brand;

public class UpdateBrandCommandValidatorTest
{
    [Theory]
    [MemberData(
        nameof(UpdateBrandValidationTestCases.InvalidNameCases),
        MemberType = typeof(UpdateBrandValidationTestCases))]
    public void UpdateBrandCommandValidator_Should_ReturnError_When_NameValidationFails(string? name)
    {
        // Arrange
        var validator = new UpdateBrandCommandValidator();
        var brandId = BrandId.Create(Guid.NewGuid());
        var command = new UpdateBrandCommand(brandId, name!, "https://res.cloudinary.com/over-clocked/image.png");

        // Act
        TestValidationResult<UpdateBrandCommand> result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateBrandValidationTestCases.InvalidImageUrlCases),
        MemberType = typeof(UpdateBrandValidationTestCases))]
    public void UpdateBrandCommandValidator_Should_ReturnError_When_ImageUrlValidationFails(string? imageUrl)
    {
        // Arrange
        var validator = new UpdateBrandCommandValidator();
        var brandId = BrandId.Create(Guid.NewGuid());
        var command = new UpdateBrandCommand(brandId, "Brand Name", imageUrl!);

        // Act
        TestValidationResult<UpdateBrandCommand> result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl).Only();
    }

    [Fact]
    public async Task UpdateBrandCommandValidator_Should_ReturnError_When_AllFieldsAreInvalid()
    {
        // Arrange
        var validator = new UpdateBrandCommandValidator();
        var brandId = BrandId.Create(Guid.NewGuid());
        var command = new UpdateBrandCommand(brandId, string.Empty, "imageUrl");

        // Act
        TestValidationResult<UpdateBrandCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public async Task UpdateBrandCommandValidator_Should_Pass_When_AllFieldsAreValid()
    {
        // Arrange
        var validator = new UpdateBrandCommandValidator();
        var brandId = BrandId.Create(Guid.NewGuid());
        var command = new UpdateBrandCommand(
            brandId,
            "Brand Name",
            "https://res.cloudinary.com/over-clocked/image.png");

        // Act
        TestValidationResult<UpdateBrandCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count.ShouldBe(0);
    }
}
