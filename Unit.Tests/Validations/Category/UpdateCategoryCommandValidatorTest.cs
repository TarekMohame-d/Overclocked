using Application.Features.Category.Commands.UpdateCategory;
using Shouldly;
using Unit.Tests.Validations.Category.TestCases;

namespace Unit.Tests.Validations.Category;

public class UpdateCategoryCommandValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateCategoryValidationTestCases.InvalidIdCases), MemberType = typeof(UpdateCategoryValidationTestCases))]
    public void Handle_WhenIdValidationFails_ShouldReturnError(Guid? id)
    {
        // Arrange
        var command = new UpdateCategoryWithIdCommand
        {
            Id = (Guid)id!,
            Name = "Nike",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        var validator = new UpdateCategoryCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Id").ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(UpdateCategoryValidationTestCases.InvalidNameCases), MemberType = typeof(UpdateCategoryValidationTestCases))]
    public void Handle_WhenNameValidationFails_ShouldReturnError(string? name)
    {
        // Arrange
        var command = new UpdateCategoryWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = name!,
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        var validator = new UpdateCategoryCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Name").ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(UpdateCategoryValidationTestCases.InvalidImageUrlCases), MemberType = typeof(UpdateCategoryValidationTestCases))]
    public void Handle_WhenImageUrlValidationFails_ShouldReturnError(string? imageUrl)
    {
        // Arrange
        var command = new UpdateCategoryWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageUrl = imageUrl!
        };

        var validator = new UpdateCategoryCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "ImageUrl").ShouldBeTrue();
    }
}
