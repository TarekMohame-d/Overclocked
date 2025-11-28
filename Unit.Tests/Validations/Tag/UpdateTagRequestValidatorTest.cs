using Application.Services.Tag.DTOs.Request;
using Application.Services.Tag.Validations;
using FluentValidation.TestHelper;
using Shouldly;
using Unit.Tests.Validations.Tag.TestCases;

namespace Unit.Tests.Validations.Tag;

public class UpdateTagRequestValidatorTest
{
    [Theory]
    [MemberData(
        nameof(UpdateTagValidationTestCases.InvalidNameCases),
        MemberType = typeof(UpdateTagValidationTestCases)
    )]
    public void UpdateTagRequestValidator_Should_ReturnError_When_NameValidationFails(string? name)
    {
        // Arrange
        var request = new UpdateTagRequestBody { Name = name! };

        var validator = new UpdateTagRequestValidator();

        // Act
        TestValidationResult<UpdateTagRequestBody> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Name).Only();
    }
}
