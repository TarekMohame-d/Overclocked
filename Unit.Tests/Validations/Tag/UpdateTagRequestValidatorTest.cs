using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.Validations;
using Shouldly;
using Unit.Tests.Validations.Tag.TestCases;

namespace Unit.Tests.Validations.Tag;

public class UpdateTagRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(UpdateTagValidationTestCases.InvalidNameCases), MemberType = typeof(UpdateTagValidationTestCases))]
    public void Handle_WhenNameValidationFails_ShouldReturnError(string? name)
    {
        // Arrange
        var command = new UpdateTagRequest
        {
            Id = Guid.CreateVersion7(),
            Name = name!
        };

        var validator = new UpdateTagRequestValidator();

        // Act
        var result = validator.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.Errors.All(e => e.PropertyName == "Name").ShouldBeTrue();
    }
}
