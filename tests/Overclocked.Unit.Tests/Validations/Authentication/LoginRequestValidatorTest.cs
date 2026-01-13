using FluentValidation.TestHelper;
using Overclocked.Application.Features.AuthenticationUseCases.Login;
using Overclocked.Unit.Tests.Validations.Authentication.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Authentication;

public class LoginRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(LoginValidationTestCases.InvalidEmailCases), MemberType = typeof(LoginValidationTestCases))]
    public void LoginRequestValidator_Should_ReturnError_When_EmailIsInvalid(string? email)
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest
        {
            Email = email!,
            Password = "P@ssword1",
            DeviceId = Guid.NewGuid(),
        };

        // Act
        TestValidationResult<LoginRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Email).Only();
    }

    [Theory]
    [MemberData(nameof(LoginValidationTestCases.InvalidPasswordCases), MemberType = typeof(LoginValidationTestCases))]
    public void LoginRequestValidator_Should_ReturnError_When_PasswordIsInvalid(string? password)
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest
        {
            Email = "email@gmail.com",
            Password = password!,
            DeviceId = Guid.NewGuid(),
        };

        // Act
        TestValidationResult<LoginRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Theory]
    [MemberData(nameof(LoginValidationTestCases.InvalidDeviceIdCases), MemberType = typeof(LoginValidationTestCases))]
    public void LoginRequestValidator_Should_ReturnError_When_DeviceIdIsInvalid(Guid deviceId)
    {
        // Arrange
        var validator = new LoginRequestValidator();
        var request = new LoginRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword1",
            DeviceId = deviceId,
        };

        // Act
        TestValidationResult<LoginRequest> result = validator.TestValidate(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.DeviceId).Only();
    }
}
