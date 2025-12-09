using FluentValidation.TestHelper;
using Overclocked.Application.Authentication.Commands.ResendEmailConfirmationCode;
using Overclocked.Unit.Tests.Validations.Authentication.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Authentication;

public class ResendEmailConfirmationCodeCommandValidatorTest
{
    [Theory]
    [MemberData(
        nameof(ResendEmailConfirmationCodeValidationTestCases.InvalidEmailCases),
        MemberType = typeof(ResendEmailConfirmationCodeValidationTestCases))]
    public void ResendEmailConfirmationCodeCommandValidator_Should_ReturnError_When_EmailIsInvalid(string? email)
    {
        // Arrange
        var validator = new ResendEmailConfirmationCodeCommandValidator();
        var request = new ResendEmailConfirmationCodeCommand(email!);

        // Act
        TestValidationResult<ResendEmailConfirmationCodeCommand> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Email).Only();
    }
}
