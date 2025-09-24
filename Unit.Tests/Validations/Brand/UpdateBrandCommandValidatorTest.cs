using Application.Features.Brand.Commands.UpdateBrand;
using Shouldly;
using Unit.Tests.Validations.Brand.TestCases;

namespace Unit.Tests.Validations.Brand;

public class UpdateBrandCommandValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateBrandValidationTestCases.InvalidIdCases), MemberType = typeof(UpdateBrandValidationTestCases))]
    public void Handle_WhenIdValidationFails_ShouldReturnError(Guid? id)
    {
        // Arrange
        var command = new UpdateBrandWithIdCommand
        {
            Id = (Guid)id!,
            Name = "Nike",
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        var validator = new UpdateBrandCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Id").ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(UpdateBrandValidationTestCases.InvalidNameCases), MemberType = typeof(UpdateBrandValidationTestCases))]
    public void Handle_WhenNameValidationFails_ShouldReturnError(string? name)
    {
        // Arrange
        var command = new UpdateBrandWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = name!,
            ImageUrl = "https://res.cloudinary.com/over-clocked/image.png"
        };

        var validator = new UpdateBrandCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Name").ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(UpdateBrandValidationTestCases.InvalidImageUrlCases), MemberType = typeof(UpdateBrandValidationTestCases))]
    public void Handle_WhenImageUrlValidationFails_ShouldReturnError(string? imageUrl)
    {
        // Arrange
        var command = new UpdateBrandWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = "Nike",
            ImageUrl = imageUrl!
        };

        var validator = new UpdateBrandCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "ImageUrl").ShouldBeTrue();
    }
}
