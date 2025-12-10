using FluentValidation.TestHelper;
using Overclocked.Application.Authentication.Commands.Login;
using Overclocked.Unit.Tests.Validations.Authentication.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Authentication;

public class LoginCommandValidatorTest
{
    [Theory]
    [MemberData(nameof(LoginValidationTestCases.InvalidEmailCases), MemberType = typeof(LoginValidationTestCases))]
    public void LoginCommandValidator_Should_ReturnError_When_EmailIsInvalid(string? email)
    {
        // Arrange
        var validator = new LoginCommandValidator();
        var request = new LoginCommand
        {
            Email = email!,
            Password = "P@ssword1",
            DeviceId = "device-id"
        };

        // Act
        TestValidationResult<LoginCommand> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Email).Only();
    }

    [Theory]
    [MemberData(nameof(LoginValidationTestCases.InvalidPasswordCases), MemberType = typeof(LoginValidationTestCases))]
    public void LoginCommandValidator_Should_ReturnError_When_PasswordIsInvalid(string? password)
    {
        // Arrange
        var validator = new LoginCommandValidator();
        var request = new LoginCommand
        {
            Email = "email@gmail.com",
            Password = password!,
            DeviceId = "device-id"
        };

        // Act
        TestValidationResult<LoginCommand> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Theory]
    [MemberData(nameof(LoginValidationTestCases.InvalidDeviceIdCases), MemberType = typeof(LoginValidationTestCases))]
    public void LoginCommandValidator_Should_ReturnError_When_DeviceIdIsInvalid(string? deviceId)
    {
        // Arrange
        var validator = new LoginCommandValidator();
        var request = new LoginCommand
        {
            Email = "email@gmail.com",
            Password = "P@ssword1",
            DeviceId = deviceId!
        };

        // Act
        TestValidationResult<LoginCommand> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.DeviceId).Only();
    }
}
