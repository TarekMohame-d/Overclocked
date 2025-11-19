using System.Linq.Expressions;
using Application.Abstraction.Repositories;
using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.Validations;
using FluentValidation.TestHelper;
using NSubstitute;
using Shouldly;
using Unit.Tests.Validations.Authentication.TestCases;
using UserEntity = Domain.Entities.User;

namespace Unit.Tests.Validations.Authentication;

public class RegisterRequestValidatorTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    [Theory]
    [MemberData(
        nameof(RegisterValidationTestCases.InvalidFirstNameCases),
        MemberType = typeof(RegisterValidationTestCases)
    )]
    public async Task RegisterRequestValidator_Should_ReturnError_When_FirstNameIsInvalid(string? firstName)
    {
        // Arrange
        var validator = new RegisterRequestValidator(_userRepository);
        var request = new RegisterRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword1",
            PhoneNumber = "1234567890",
            FirstName = firstName!,
            LastName = "last name",
        };

        _userRepository
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<RegisterRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.FirstName).Only();
    }

    [Theory]
    [MemberData(
        nameof(RegisterValidationTestCases.InvalidLastNameCases),
        MemberType = typeof(RegisterValidationTestCases)
    )]
    public async Task RegisterRequestValidator_Should_ReturnError_When_LastNameIsInvalid(string? lastName)
    {
        // Arrange
        var validator = new RegisterRequestValidator(_userRepository);
        var request = new RegisterRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword1",
            PhoneNumber = "1234567890",
            FirstName = "first name",
            LastName = lastName!,
        };

        _userRepository
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<RegisterRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.LastName).Only();
    }

    [Theory]
    [MemberData(
        nameof(RegisterValidationTestCases.InvalidEmailCases),
        MemberType = typeof(RegisterValidationTestCases)
    )]
    public async Task RegisterRequestValidator_Should_ReturnError_When_EmailIsInvalid(string? email)
    {
        // Arrange
        var validator = new RegisterRequestValidator(_userRepository);
        var request = new RegisterRequest
        {
            Email = email!,
            Password = "P@ssword1",
            PhoneNumber = "1234567890",
            FirstName = "first name",
            LastName = "last name",
        };

        _userRepository
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<RegisterRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Email).Only();
    }

    [Fact]
    public async Task RegisterRequestValidator_Should_ReturnError_When_EmailAlreadyExists()
    {
        // Arrange
        var validator = new RegisterRequestValidator(_userRepository);
        var request = new RegisterRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword1",
            PhoneNumber = "1234567890",
            FirstName = "first name",
            LastName = "last name",
        };

        _userRepository
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        TestValidationResult<RegisterRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();

        await _userRepository
            .Received(2)
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [MemberData(
        nameof(RegisterValidationTestCases.InvalidPasswordCases),
        MemberType = typeof(RegisterValidationTestCases)
    )]
    public async Task RegisterRequestValidator_Should_ReturnError_When_PasswordIsInvalid(string? password)
    {
        // Arrange
        var validator = new RegisterRequestValidator(_userRepository);
        var request = new RegisterRequest
        {
            Email = "email@gmail.com",
            Password = password!,
            PhoneNumber = "1234567890",
            FirstName = "first name",
            LastName = "last name",
        };

        _userRepository
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<RegisterRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Theory]
    [MemberData(
        nameof(RegisterValidationTestCases.InvalidPhoneNumberCases),
        MemberType = typeof(RegisterValidationTestCases)
    )]
    public async Task RegisterRequestValidator_Should_ReturnError_When_PhoneNumberIsInvalid(string? phoneNumber)
    {
        // Arrange
        var validator = new RegisterRequestValidator(_userRepository);
        var request = new RegisterRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword1",
            PhoneNumber = phoneNumber!,
            FirstName = "first name",
            LastName = "last name",
        };

        _userRepository
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<RegisterRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber).Only();
    }

    [Fact]
    public async Task RegisterRequestValidator_Should_ReturnError_When_PhoneNumberAlreadyExists()
    {
        // Arrange
        var validator = new RegisterRequestValidator(_userRepository);
        var request = new RegisterRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword1",
            PhoneNumber = "1234567890",
            FirstName = "first name",
            LastName = "last name",
        };

        _userRepository
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        TestValidationResult<RegisterRequest> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();

        await _userRepository
            .Received(2)
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>());
    }
}
