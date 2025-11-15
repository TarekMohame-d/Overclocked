using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.Validations;
using FluentValidation.TestHelper;
using Shouldly;
using Unit.Tests.Validations.Authentication.TestCases;

namespace Unit.Tests.Validations.Authentication;

public class ResendEmailConfirmationCodeRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(ResendEmailConfirmationCodeValidationTestCases.InvalidEmailCases),
        MemberType = typeof(ResendEmailConfirmationCodeValidationTestCases))]
    public void ForgetPasswordRequestValidator_Should_ReturnError_When_EmailIsInvalid(string? email)
    {
        // Arrange
        var validator = new ResendEmailConfirmationCodeRequestValidator();
        var request = new ResendEmailConfirmationCodeRequest
        {
            Email = email!
        };

        // Act
        TestValidationResult<ResendEmailConfirmationCodeRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Email).Only();
    }
}
