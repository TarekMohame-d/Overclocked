using System.Linq.Expressions;
using System.Net;
using Application.Abstraction.DomainServices;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Services.Authentication;
using Application.Services.Authentication.Events;
using Application.Services.Authentication.Helpers.Interfaces;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;
using LoginRequest = Application.Services.Authentication.DTOs.Request.LoginRequest;

namespace Unit.Tests.AuthenticationTests;

public class LoginAsyncTest
{
    private readonly AuthenticationService _authenticationService;
    private readonly IEmailConfirmationCodeHasher _emailConfirmationCodeHasherMock;
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IEventDispatcher _eventDispatcherMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly IRefreshTokenService _refreshTokenServiceMock;
    private readonly ITokenProvider _tokenProviderMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IUserRepository _userRepositoryMock;
    private readonly IRolePermissionsRepository _rolePermissionsRepositoryMock;
    private readonly ITokenReaderService _tokenReaderServiceMock;
    private readonly ICartService _cartServiceMock;

    public LoginAsyncTest()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _emailConfirmationCodeHasherMock = Substitute.For<IEmailConfirmationCodeHasher>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _eventDispatcherMock = Substitute.For<IEventDispatcher>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _refreshTokenServiceMock = Substitute.For<IRefreshTokenService>();
        _tokenProviderMock = Substitute.For<ITokenProvider>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _rolePermissionsRepositoryMock = Substitute.For<IRolePermissionsRepository>();
        _tokenReaderServiceMock = Substitute.For<ITokenReaderService>();
        _cartServiceMock = Substitute.For<ICartService>();

        _authenticationService = new AuthenticationService(
            _userRepositoryMock,
            _rolePermissionsRepositoryMock,
            _unitOfWorkMock,
            _passwordHasherMock,
            _eventDispatcherMock,
            _emailConfirmationCodeHasherMock,
            _emailConfirmationCodeServiceMock,
            _tokenProviderMock,
            _refreshTokenServiceMock,
            _tokenReaderServiceMock,
            _cartServiceMock
        );
    }

    [Fact]
    public async Task LoginAsync_When_EmailNotExist_ShouldReturnFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "email@gmail.com",
            Password = "password",
            DeviceId = "device-id",
        };

        _userRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns((User)null!);

        _passwordHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        // Act
        Result result = await _authenticationService.LoginAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _userRepositoryMock
            .Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            );

        _passwordHasherMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task LoginAsync_When_PasswordIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "email@gmail.com",
            Password = "password",
            DeviceId = "device-id",
        };

        User user = new UserFaker().Generate();
        _userRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(user);

        _passwordHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        // Act
        Result result = await _authenticationService.LoginAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _userRepositoryMock
            .Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            );

        _passwordHasherMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task LoginAsync_When_EmailIsNotConfirmed_ShouldReturnFailure()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "email@gmail.com",
            Password = "password",
            DeviceId = "device-id",
        };

        User user = new UserFaker().Generate();
        user.EmailConfirmed = false;

        _userRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(user);

        _passwordHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        _eventDispatcherMock
            .DispatchAsync(Arg.Any<EmailNotConfirmedEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _authenticationService.LoginAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _userRepositoryMock
            .Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            );

        _passwordHasherMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());

        await _eventDispatcherMock
            .Received(1)
            .DispatchAsync(Arg.Any<EmailNotConfirmedEvent>(), Arg.Any<CancellationToken>());
    }
}
