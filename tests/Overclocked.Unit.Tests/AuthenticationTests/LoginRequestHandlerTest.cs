using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Features.AuthenticationUseCases.DTOs.Responses;
using Overclocked.Application.Features.AuthenticationUseCases.Login;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Infrastructure.Authentication;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.AuthenticationTests;

public class LoginRequestHandlerTest
{
    private readonly IAuthenticationRepository _authenticationRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly IRefreshTokenHasher _refreshTokenHasherMock;
    private readonly ITokenProvider _tokenProviderMock;
    private readonly LoginRequestHandler _loginRequestHandler;

    public LoginRequestHandlerTest()
    {
        _authenticationRepositoryMock = Substitute.For<IAuthenticationRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _tokenProviderMock = Substitute.For<ITokenProvider>();
        _refreshTokenHasherMock = Substitute.For<IRefreshTokenHasher>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _loginRequestHandler = new LoginRequestHandler(
            _authenticationRepositoryMock,
            _unitOfWorkMock,
            _emailConfirmationCodeServiceMock,
            _passwordHasherMock,
            _refreshTokenHasherMock,
            _tokenProviderMock
        );
    }

    [Fact]
    public async Task LoginRequestHandler_Should_ReturnFailure_When_UserWithEmailNotExist()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "email@gmail.com",
            Password = "password",
            DeviceId = Guid.NewGuid(),
        };

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User)null!);

        _passwordHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        // Act
        Result result = await _loginRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _passwordHasherMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task LoginRequestHandler_Should_ReturnFailure_When_PasswordIsInvalid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "email@gmail.com",
            Password = "password",
            DeviceId = Guid.NewGuid(),
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        _passwordHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        // Act
        Result<AuthResponse> result = await _loginRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _passwordHasherMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task LoginRequestHandler_Should_ReturnFailure_When_UserIsNotActive()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "email@gmail.com",
            Password = "password",
            DeviceId = Guid.NewGuid(),
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        user.Deactivate();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        _passwordHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        // Act
        Result<AuthResponse> result = await _loginRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Forbidden);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _passwordHasherMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task LoginRequestHandler_Should_ReturnFailure_When_EmailIsNotConfirmed()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "email@gmail.com",
            Password = "password",
            DeviceId = Guid.NewGuid(),
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        _passwordHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        // Act
        Result<AuthResponse> result = await _loginRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);
        user.DomainEvents.ShouldNotBeEmpty();

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _passwordHasherMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task LoginRequestHandler_Should_ReturnAuthResponse_When_AllDataValid()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "email@gmail.com",
            Password = "password",
            DeviceId = Guid.NewGuid(),
        };

        _tokenProviderMock.GenerateRefreshToken().Returns("refresh-token");

        _refreshTokenHasherMock.Hash(Arg.Any<string>()).Returns("refresh-token-hash");

        User user = new UserFaker(new PasswordHasher()).Generate();

        user.ConfirmEmail();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        _passwordHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        _authenticationRepositoryMock.GetPermissionsAsync(Arg.Any<Role>(), Arg.Any<CancellationToken>()).Returns([]);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result<AuthResponse> result = await _loginRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _passwordHasherMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());

        await _authenticationRepositoryMock.Received(1).GetPermissionsAsync(Arg.Any<Role>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
