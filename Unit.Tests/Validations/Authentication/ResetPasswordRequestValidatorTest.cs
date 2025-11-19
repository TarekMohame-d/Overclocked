using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.Validations;
using FluentValidation.TestHelper;
using Shouldly;
using Unit.Tests.Validations.Authentication.TestCases;

namespace Unit.Tests.Validations.Authentication;

public class ResetPasswordRequestValidatorTest
{
    [Theory]
    [MemberData(
        nameof(ResetPasswordValidationTestCases.InvalidEmailCases),
        MemberType = typeof(ResetPasswordValidationTestCases)
    )]
    public void ResetPasswordRequestValidator_Should_ReturnError_When_EmailIsInvalid(string? email)
    {
        // Arrange
        var validator = new ResetPasswordRequestValidator();
        var request = new ResetPasswordRequest
        {
            Email = email!,
            Password = "P@ssword1",
            Code = "VC74A1",
        };

        // Act
        TestValidationResult<ResetPasswordRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Email).Only();
    }

    [Theory]
    [MemberData(
        nameof(ResetPasswordValidationTestCases.InvalidPasswordCases),
        MemberType = typeof(ResetPasswordValidationTestCases)
    )]
    public void ResetPasswordRequestValidator_Should_ReturnError_When_PasswordIsInvalid(string? password)
    {
        // Arrange
        var validator = new ResetPasswordRequestValidator();
        var request = new ResetPasswordRequest
        {
            Email = "email@gmail.com",
            Password = password!,
            Code = "VC74A1",
        };

        // Act
        TestValidationResult<ResetPasswordRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Theory]
    [MemberData(
        nameof(ResetPasswordValidationTestCases.InvalidCodeCases),
        MemberType = typeof(ResetPasswordValidationTestCases)
    )]
    public void ResetPasswordRequestValidator_Should_ReturnError_When_CodeIsInvalid(string? code)
    {
        // Arrange
        var validator = new ResetPasswordRequestValidator();
        var request = new ResetPasswordRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword1",
            Code = code!,
        };

        // Act
        TestValidationResult<ResetPasswordRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Code).Only();
    }
}
