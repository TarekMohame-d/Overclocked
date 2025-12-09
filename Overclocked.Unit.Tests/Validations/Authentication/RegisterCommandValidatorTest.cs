using System.Linq.Expressions;
using FluentValidation.TestHelper;
using NSubstitute;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Authentication.Commands.Register;
using Overclocked.Unit.Tests.Validations.Authentication.TestCases;
using Shouldly;
using UserEntity = Overclocked.Domain.UserAggregate.User;

namespace Overclocked.Unit.Tests.Validations.Authentication;

public class RegisterCommandValidatorTest
{
    private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

    [Theory]
    [MemberData(
        nameof(RegisterValidationTestCases.InvalidFirstNameCases),
        MemberType = typeof(RegisterValidationTestCases))]
    public async Task RegisterCommandValidator_Should_ReturnError_When_FirstNameIsInvalid(string? firstName)
    {
        // Arrange
        var validator = new RegisterCommandValidator(_userRepository);
        var request = new RegisterCommand("email@gmail.com", "P@ssword1", firstName!, "last name", "01234567890");

        _userRepository
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<RegisterCommand> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.FirstName).Only();
    }

    [Theory]
    [MemberData(
        nameof(RegisterValidationTestCases.InvalidLastNameCases),
        MemberType = typeof(RegisterValidationTestCases))]
    public async Task RegisterCommandValidator_Should_ReturnError_When_LastNameIsInvalid(string? lastName)
    {
        // Arrange
        var validator = new RegisterCommandValidator(_userRepository);
        var request = new RegisterCommand("email@gmail.com", "P@ssword1", "first name", lastName!, "01234567890");

        _userRepository
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<RegisterCommand> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.LastName).Only();
    }

    [Theory]
    [MemberData(
        nameof(RegisterValidationTestCases.InvalidEmailCases),
        MemberType = typeof(RegisterValidationTestCases))]
    public async Task RegisterCommandValidator_Should_ReturnError_When_EmailIsInvalid(string? email)
    {
        // Arrange
        var validator = new RegisterCommandValidator(_userRepository);
        var request = new RegisterCommand(email!, "P@ssword1", "first name", "last name", "01234567890");

        _userRepository
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<RegisterCommand> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Email).Only();
    }

    [Fact]
    public async Task RegisterCommandValidator_Should_ReturnError_When_EmailAlreadyExists()
    {
        // Arrange
        var validator = new RegisterCommandValidator(_userRepository);
        var request = new RegisterCommand("email@gmail.com", "P@ssword1", "first name", "last name", "01234567890");

        _userRepository.AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        TestValidationResult<RegisterCommand> result = await validator.TestValidateAsync(request);

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
    public async Task RegisterCommandValidator_Should_ReturnError_When_PasswordIsInvalid(string? password)
    {
        // Arrange
        var validator = new RegisterCommandValidator(_userRepository);
        var request = new RegisterCommand("email@gmail.com", password!, "first name", "last name", "01234567890");

        _userRepository
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<RegisterCommand> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.Password).Only();
    }

    [Theory]
    [MemberData(
        nameof(RegisterValidationTestCases.InvalidPhoneNumberCases),
        MemberType = typeof(RegisterValidationTestCases))]
    public async Task RegisterCommandValidator_Should_ReturnError_When_PhoneNumberIsInvalid(string? phoneNumber)
    {
        // Arrange
        var validator = new RegisterCommandValidator(_userRepository);
        var request = new RegisterCommand("email@gmail.com", "P@ssword1", "first name", "last name", phoneNumber!);

        _userRepository
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        TestValidationResult<RegisterCommand> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();
        result.ShouldHaveValidationErrorFor(x => x.PhoneNumber).Only();
    }

    [Fact]
    public async Task RegisterCommandValidator_Should_ReturnError_When_PhoneNumberAlreadyExists()
    {
        // Arrange
        var validator = new RegisterCommandValidator(_userRepository);
        var request = new RegisterCommand("email@gmail.com", "P@ssword2", "first name", "last name", "01234567890");

        _userRepository
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(true);

        // Act
        TestValidationResult<RegisterCommand> result = await validator.TestValidateAsync(request);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldNotBeEmpty();

        await _userRepository
            .Received(2)
            .AnyAsync(Arg.Any<Expression<Func<UserEntity, bool>>>(), Arg.Any<CancellationToken>());
    }
}
