using FluentValidation.TestHelper;
using Overclocked.Application.Features.AuthenticationUseCases.ResendEmailConfirmationCode;
using Overclocked.Unit.Tests.Validations.Authentication.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Authentication;

public class ResendEmailConfirmationCodeRequestValidatorTest
{
    [Theory]
    [MemberData(
        nameof(ResendEmailConfirmationCodeValidationTestCases.InvalidEmailCases),
        MemberType = typeof(ResendEmailConfirmationCodeValidationTestCases)
    )]
    public void ResendEmailConfirmationCodeRequestValidator_Should_ReturnError_When_EmailIsInvalid(string? email)
    {
        // Arrange
        var validator = new ResendEmailConfirmationCodeRequestValidator();
        var request = new ResendEmailConfirmationCodeRequest { Email = email! };

        // Act
        TestValidationResult<ResendEmailConfirmationCodeRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Email).Only();
    }
}
