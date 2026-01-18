using FluentValidation.TestHelper;
using Overclocked.Application.Features.AuthenticationUseCases.Register;
using Overclocked.Unit.Tests.Validations.Authentication.TestCases;
using Shouldly;

namespace Overclocked.Unit.Tests.Validations.Authentication;

public class RegisterRequestValidatorTest
{
    [Theory]
    [MemberData(nameof(RegisterValidationTestCases.InvalidFirstNameCases), MemberType = typeof(RegisterValidationTestCases))]
    public async Task RegisterRequestValidator_Should_ReturnError_When_FirstNameIsInvalid(string? firstName)
    {
        // Arrange
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword1",
            FirstName = firstName!,
            LastName = "last name",
            PhoneNumber = "01234567890",
        };

        // Act
        TestValidationResult<RegisterRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.FirstName).Only();
    }

    [Theory]
    [MemberData(nameof(RegisterValidationTestCases.InvalidLastNameCases), MemberType = typeof(RegisterValidationTestCases))]
    public async Task RegisterRequestValidator_Should_ReturnError_When_LastNameIsInvalid(string? lastName)
    {
        // Arrange
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword1",
            FirstName = "first name",
            LastName = lastName!,
            PhoneNumber = "01234567890",
        };

        // Act
        TestValidationResult<RegisterRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.LastName).Only();
    }

    [Theory]
    [MemberData(nameof(RegisterValidationTestCases.InvalidEmailCases), MemberType = typeof(RegisterValidationTestCases))]
    public async Task RegisterRequestValidator_Should_ReturnError_When_EmailIsInvalid(string? email)
    {
        // Arrange
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Email = email!,
            Password = "P@ssword1",
            FirstName = "first name",
            LastName = "last name",
            PhoneNumber = "01234567890",
        };

        // Act
        TestValidationResult<RegisterRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Email).Only();
    }

    [Theory]
    [MemberData(nameof(RegisterValidationTestCases.InvalidPasswordCases), MemberType = typeof(RegisterValidationTestCases))]
    public async Task RegisterRequestValidator_Should_ReturnError_When_PasswordIsInvalid(string? password)
    {
        // Arrange
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Email = "email2@gmail.com",
            Password = password!,
            FirstName = "first name",
            LastName = "last name",
            PhoneNumber = "01234567890",
        };

        // Act
        TestValidationResult<RegisterRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Theory]
    [MemberData(nameof(RegisterValidationTestCases.InvalidPhoneNumberCases), MemberType = typeof(RegisterValidationTestCases))]
    public async Task RegisterRequestValidator_Should_ReturnError_When_PhoneNumberIsInvalid(string? phoneNumber)
    {
        // Arrange
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            Email = "email2@gmail.com",
            Password = "P@ssword1",
            FirstName = "first name",
            LastName = "last name",
            PhoneNumber = phoneNumber!,
        };

        // Act
        TestValidationResult<RegisterRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber).Only();
    }
}
