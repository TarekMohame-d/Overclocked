using FluentValidation.TestHelper;
using Overclocked.Application.Features.AuthenticationUseCases.ForgetPassword;
using Overclocked.Unit.Tests.Validations.Authentication.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Authentication;

public class ForgetPasswordRequestValidatorTest
{
    [Theory]
    [MemberData(
        nameof(ForgetPasswordValidationTestCases.InvalidEmailCases),
        MemberType = typeof(ForgetPasswordValidationTestCases)
    )]
    public void ForgetPasswordRequestValidator_Should_ReturnError_When_EmailIsInvalid(string? email)
    {
        // Arrange
        var validator = new ForgetPasswordRequestValidator();
        var request = new ForgetPasswordRequest { Email = email! };

        // Act
        TestValidationResult<ForgetPasswordRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Email).Only();
    }
}
