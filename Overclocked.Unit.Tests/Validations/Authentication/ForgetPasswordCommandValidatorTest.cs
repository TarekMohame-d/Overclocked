using FluentValidation.TestHelper;
using Overclocked.Application.Authentication.Commands.ForgetPassword;
using Overclocked.Unit.Tests.Validations.Authentication.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Authentication;

public class ForgetPasswordCommandValidatorTest
{
    [Theory]
    [MemberData(
        nameof(ForgetPasswordValidationTestCases.InvalidEmailCases),
        MemberType = typeof(ForgetPasswordValidationTestCases))]
    public void ForgetPasswordCommandValidator_Should_ReturnError_When_EmailIsInvalid(string? email)
    {
        // Arrange
        var validator = new ForgetPasswordCommandValidator();
        var request = new ForgetPasswordCommand(email!);

        // Act
        TestValidationResult<ForgetPasswordCommand> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Email).Only();
    }
}
