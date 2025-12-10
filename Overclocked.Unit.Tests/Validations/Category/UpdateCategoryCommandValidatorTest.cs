using FluentValidation.TestHelper;
using Overclocked.Application.Category.Commands.UpdateCategory;
using Overclocked.Domain.CategoryAggregate.ValueObjects;
using Overclocked.Unit.Tests.Validations.Category.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Category;

public class UpdateCategoryCommandValidatorTest
{
    [Theory]
    [MemberData(
        nameof(UpdateCategoryValidationTestCases.InvalidNameCases),
        MemberType = typeof(UpdateCategoryValidationTestCases))]
    public void UpdateCategoryCommandValidator_Should_ReturnError_When_NameValidationFails(string? name)
    {
        // Arrange
        var validator = new UpdateCategoryCommandValidator();
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand
        {
            Id = categoryId,
            Name = name!,
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        // Act
        TestValidationResult<UpdateCategoryCommand> result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }

    [Theory]
    [MemberData(
        nameof(UpdateCategoryValidationTestCases.InvalidImageUrlCases),
        MemberType = typeof(UpdateCategoryValidationTestCases))]
    public void UpdateCategoryCommandValidator_Should_ReturnError_When_ImageUrlValidationFails(string? imageUrl)
    {
        // Arrange
        var validator = new UpdateCategoryCommandValidator();
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand
        {
            Id = categoryId,
            Name = "Category Name",
            ImageUrl = imageUrl!
        };

        // Act
        TestValidationResult<UpdateCategoryCommand> result = validator.TestValidate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.ImageUrl).Only();
    }

    [Fact]
    public async Task UpdateCategoryCommandValidator_Should_ReturnError_When_AllFieldsAreInvalid()
    {
        // Arrange
        var validator = new UpdateCategoryCommandValidator();
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand
        {
            Id = categoryId,
            Name = string.Empty,
            ImageUrl = "imageUrl"
        };

        // Act
        TestValidationResult<UpdateCategoryCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.Count.ShouldBe(2);
    }

    [Fact]
    public async Task UpdateCategoryCommandValidator_Should_Pass_When_AllFieldsAreValid()
    {
        // Arrange
        var validator = new UpdateCategoryCommandValidator();
        var categoryId = Guid.NewGuid();
        var command = new UpdateCategoryCommand
        {
            Id = categoryId,
            Name = "Category Name",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        // Act
        TestValidationResult<UpdateCategoryCommand> result = await validator.TestValidateAsync(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
        result.Errors.Count.ShouldBe(0);
    }
}
