using Application.Features.Tag.Commands.UpdateTag;
using Shouldly;
using Unit.Tests.Validations.Tag.TestCases;

namespace Unit.Tests.Validations.Tag;

public class UpdateTagCommandValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateTagValidationTestCases.InvalidIdCases), MemberType = typeof(UpdateTagValidationTestCases))]
    public void Handle_WhenIdValidationFails_ShouldReturnError(Guid? id)
    {
        // Arrange
        var command = new UpdateTagWithIdCommand
        {
            Id = (Guid)id!,
            Name = "Nike"
        };

        var validator = new UpdateTagCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Id").ShouldBeTrue();
    }

    [Theory]
    [MemberData(nameof(UpdateTagValidationTestCases.InvalidNameCases), MemberType = typeof(UpdateTagValidationTestCases))]
    public void Handle_WhenNameValidationFails_ShouldReturnError(string? name)
    {
        // Arrange
        var command = new UpdateTagWithIdCommand
        {
            Id = Guid.CreateVersion7(),
            Name = name!
        };

        var validator = new UpdateTagCommandValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Name").ShouldBeTrue();
    }
}
