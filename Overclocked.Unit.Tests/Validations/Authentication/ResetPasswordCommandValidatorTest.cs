using FluentValidation.TestHelper;
using Overclocked.Application.Authentication.Commands.ResetPassword;
using Overclocked.Unit.Tests.Validations.Authentication.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Authentication;

public class ResetPasswordCommandValidatorTest
{
    [Theory]
    [MemberData(
        nameof(ResetPasswordValidationTestCases.InvalidEmailCases),
        MemberType = typeof(ResetPasswordValidationTestCases))]
    public void ResetPasswordCommandValidator_Should_ReturnError_When_EmailIsInvalid(string? email)
    {
        // Arrange
        var validator = new ResetPasswordCommandValidator();
        var request = new ResetPasswordCommand
        {
            Email = email!,
            Password = "P@ssword1",
            Code = "VC74A1"
        };

        // Act
        TestValidationResult<ResetPasswordCommand> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Email).Only();
    }

    [Theory]
    [MemberData(
        nameof(ResetPasswordValidationTestCases.InvalidPasswordCases),
        MemberType = typeof(ResetPasswordValidationTestCases))]
    public void ResetPasswordCommandValidator_Should_ReturnError_When_PasswordIsInvalid(string? password)
    {
        // Arrange
        var validator = new ResetPasswordCommandValidator();
        var request = new ResetPasswordCommand
        {
            Email = "email@gmail.com",
            Password = password!,
            Code = "VC74A1"
        };

        // Act
        TestValidationResult<ResetPasswordCommand> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Theory]
    [MemberData(
        nameof(ResetPasswordValidationTestCases.InvalidCodeCases),
        MemberType = typeof(ResetPasswordValidationTestCases))]
    public void ResetPasswordCommandValidator_Should_ReturnError_When_CodeIsInvalid(string? code)
    {
        // Arrange
        var validator = new ResetPasswordCommandValidator();
        var request = new ResetPasswordCommand
        {
            Email = "email@gmail.com",
            Password = "P@ssword1",
            Code = code!
        };

        // Act
        TestValidationResult<ResetPasswordCommand> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Code).Only();
    }
}
