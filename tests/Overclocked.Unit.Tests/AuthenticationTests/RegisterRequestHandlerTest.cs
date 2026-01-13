using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Features.AuthenticationUseCases.Register;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.AuthenticationTests;

public class RegisterRequestHandlerTest
{
    private readonly IAuthenticationRepository _authenticationRepositoryMock;
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly RegisterRequestHandler _registerRequestHandler;

    public RegisterRequestHandlerTest()
    {
        _authenticationRepositoryMock = Substitute.For<IAuthenticationRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _registerRequestHandler = new RegisterRequestHandler(
            _authenticationRepositoryMock,
            _unitOfWorkMock,
            _passwordHasherMock,
            _emailConfirmationCodeServiceMock
        );
    }

    [Fact]
    public async Task RegisterRequestHandler_Should_ReturnFailure_When_PhoneAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "email@gmail.com",
            Password = "password",
            FirstName = "first name",
            LastName = "last name",
            PhoneNumber = "1234567890",
        };

        _authenticationRepositoryMock.PhoneExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Result result = await _registerRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.Description.ShouldContain(AuthenticationErrors.PhoneAlreadyExists.Description);

        await _authenticationRepositoryMock.Received(1).PhoneExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _authenticationRepositoryMock.DidNotReceive().Add(Arg.Any<User>());

        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterRequestHandler_Should_ReturnFailure_When_EmailAlreadyExists()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "email@gmail.com",
            Password = "password",
            FirstName = "first name",
            LastName = "last name",
            PhoneNumber = "1234567890",
        };

        _authenticationRepositoryMock.PhoneExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _authenticationRepositoryMock.EmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);

        // Act
        Result result = await _registerRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.Description.ShouldContain(AuthenticationErrors.EmailAlreadyExists.Description);

        await _authenticationRepositoryMock.Received(1).EmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterRequestHandler_Should_ReturnFailure_When_OneValidationFails()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "wrong email",
            Password = "P@ssword123",
            FirstName = "first name",
            LastName = "last name",
            PhoneNumber = "1234567890",
        };

        _authenticationRepositoryMock.PhoneExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _authenticationRepositoryMock.EmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        // Act
        Result result = await _registerRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        _authenticationRepositoryMock.DidNotReceive().Add(Arg.Any<User>());

        await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RegisterRequestHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword123",
            FirstName = "first name",
            LastName = "last name",
            PhoneNumber = "1234567890",
        };

        _authenticationRepositoryMock.PhoneExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        _authenticationRepositoryMock.EmailExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        User user = new UserFaker(new PasswordHasher()).Generate();

        _authenticationRepositoryMock.Add(Arg.Any<User>());

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _registerRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        user.DomainEvents.ShouldNotBeEmpty();
        user.EmailConfirmationCode.ShouldNotBeNull();

        _authenticationRepositoryMock.Received(1).Add(Arg.Any<User>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
