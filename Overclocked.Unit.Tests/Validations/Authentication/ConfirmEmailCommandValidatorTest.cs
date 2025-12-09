using FluentValidation.TestHelper;
using Overclocked.Application.Authentication.Commands.ConfirmEmail;
using Overclocked.Unit.Tests.Validations.Authentication.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Authentication;

public class ConfirmEmailCommandValidatorTest
{
    [Theory]
    [MemberData(
        nameof(ConfirmEmailValidationTestCases.InvalidEmailCases),
        MemberType = typeof(ConfirmEmailValidationTestCases))]
    public void ConfirmEmailCommandValidator_Should_ReturnError_When_EmailIsInvalid(string? email)
    {
        // Arrange
        var validator = new ConfirmEmailCommandValidator();
        var request = new ConfirmEmailCommand
        {
            Email = email!,
            Code = "VC74A1"
        };

        // Act
        TestValidationResult<ConfirmEmailCommand> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Email).Only();
    }

    [Theory]
    [MemberData(
        nameof(ConfirmEmailValidationTestCases.InvalidCodeCases),
        MemberType = typeof(ConfirmEmailValidationTestCases))]
    public void ConfirmEmailCommandValidator_Should_ReturnError_When_CodeIsInvalid(string? code)
    {
        // Arrange
        var validator = new ConfirmEmailCommandValidator();
        var request = new ConfirmEmailCommand
        {
            Email = "email@gmail.com",
            Code = code!
        };

        // Act
        TestValidationResult<ConfirmEmailCommand> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Code).Only();
    }
}
