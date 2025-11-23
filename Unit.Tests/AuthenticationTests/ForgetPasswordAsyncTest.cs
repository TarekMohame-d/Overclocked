using System.Linq.Expressions;
using System.Net;
using Application.Abstraction.DomainServices;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Services.Authentication;
using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.Events;
using Application.Services.Authentication.Helpers.Interfaces;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Domain.Exceptions;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.AuthenticationTests;

public class ForgetPasswordAsyncTest
{
    private readonly AuthenticationService _authenticationService;
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
    private readonly IWishlistService _wishlistServiceMock;

    public ForgetPasswordAsyncTest()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _eventDispatcherMock = Substitute.For<IEventDispatcher>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _refreshTokenServiceMock = Substitute.For<IRefreshTokenService>();
        _tokenProviderMock = Substitute.For<ITokenProvider>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _rolePermissionsRepositoryMock = Substitute.For<IRolePermissionsRepository>();
        _tokenReaderServiceMock = Substitute.For<ITokenReaderService>();
        _cartServiceMock = Substitute.For<ICartService>();
        _wishlistServiceMock = Substitute.For<IWishlistService>();

        _authenticationService = new AuthenticationService(
            _userRepositoryMock,
            _rolePermissionsRepositoryMock,
            _unitOfWorkMock,
            _passwordHasherMock,
            _eventDispatcherMock,
            _emailConfirmationCodeServiceMock,
            _tokenProviderMock,
            _refreshTokenServiceMock,
            _tokenReaderServiceMock,
            _cartServiceMock,
            _wishlistServiceMock
        );
    }

    [Fact]
    public async Task ForgetPasswordAsync_Should_ReturnSuccess_When_EmailNotExist()
    {
        // Arrange
        var request = new ForgetPasswordRequest { Email = "email@gmail.com" };

        _userRepositoryMock
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns((User)null!);

        // Act
        Result result = await _authenticationService.ForgetPasswordAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();

        await _userRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        await _eventDispatcherMock.DidNotReceive()
            .DispatchAsync(Arg.Any<ForgetPasswordEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ForgetPasswordAsync_Should_ReturnFailure_When_EmailConfirmationCodeNotExist()
    {
        // Arrange
        var request = new ForgetPasswordRequest { Email = "email@gmail.com" };

        User user = new UserFaker().Generate();
        _userRepositoryMock
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(user);

        _emailConfirmationCodeServiceMock
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns((EmailConfirmationCode)null!);

        // Act
        Exception exception = await Should.ThrowAsync<Exception>(async () =>
            await _authenticationService.ForgetPasswordAsync(request, CancellationToken.None));

        // Assert
        exception.ShouldBeOfType<EmailConfirmationCodeNotExistException>();

        await _userRepositoryMock
            .Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        await _emailConfirmationCodeServiceMock.Received(1)
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());

        await _eventDispatcherMock.DidNotReceive()
            .DispatchAsync(Arg.Any<ForgetPasswordEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ForgetPasswordAsync_Should_ReturnSuccess_When_AllDataValid()
    {
        // Arrange
        var request = new ForgetPasswordRequest { Email = "email@gmail.com" };

        User user = new UserFaker().Generate();
        _userRepositoryMock
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(user);

        EmailConfirmationCode emailConfirmationCode = new EmailConfirmationCodeFaker().Generate();

        _emailConfirmationCodeServiceMock
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(emailConfirmationCode);

        _emailConfirmationCodeServiceMock.UpdateEmailConfirmationCode(emailConfirmationCode)
            .Returns("");

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        _eventDispatcherMock.DispatchAsync(Arg.Any<ForgetPasswordEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        // Act
        Result result = await _authenticationService.ForgetPasswordAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBeNull();

        await _userRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        await _emailConfirmationCodeServiceMock.Received(1)
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());

        _emailConfirmationCodeServiceMock.Received(1)
            .UpdateEmailConfirmationCode(Arg.Any<EmailConfirmationCode>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _eventDispatcherMock.Received(1)
            .DispatchAsync(Arg.Any<ForgetPasswordEvent>(), Arg.Any<CancellationToken>());
    }
}
