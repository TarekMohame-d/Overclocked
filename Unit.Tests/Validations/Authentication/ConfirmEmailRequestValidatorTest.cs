using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.Validations;
using FluentValidation.TestHelper;
using Shouldly;
using Unit.Tests.Validations.Authentication.TestCases;

namespace Unit.Tests.Validations.Authentication;

public class ConfirmEmailRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(ConfirmEmailValidationTestCases.InvalidEmailCases),
        MemberType = typeof(ConfirmEmailValidationTestCases))]
    public void ConfirmEmailRequestValidator_Should_ReturnError_When_EmailIsInvalid(string? email)
    {
        // Arrange
        var validator = new ConfirmEmailRequestValidator();
        var request = new ConfirmEmailRequest
        {
            Email = email!,
            Code = "VC74A1"
        };

        // Act
        TestValidationResult<ConfirmEmailRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Email).Only();
    }

    [Theory]
    [MemberData(nameof(ConfirmEmailValidationTestCases.InvalidCodeCases),
        MemberType = typeof(ConfirmEmailValidationTestCases))]
    public void ConfirmEmailRequestValidator_Should_ReturnError_When_CodeIsInvalid(string? code)
    {
        // Arrange
        var validator = new ConfirmEmailRequestValidator();
        var request = new ConfirmEmailRequest
        {
            Email = "email@gmail.com",
            Code = code!
        };

        // Act
        TestValidationResult<ConfirmEmailRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Code).Only();
    }
}
