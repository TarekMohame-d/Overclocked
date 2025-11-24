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
using NSubstitute;
using Shouldly;

namespace Unit.Tests.AuthenticationTests;

public class ResendEmailConfirmationCodeAsyncTest
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

    public ResendEmailConfirmationCodeAsyncTest()
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
    public async Task ResendEmailConfirmationCodeAsync_Should_ReturnSuccess_When_EmailNotExist()
    {
        // Arrange
        var request = new ResendEmailConfirmationCodeRequest { Email = "email@gmail.com" };

        _userRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns((User)null!);

        // Act
        Result result = await _authenticationService.ResendEmailConfirmationCodeAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _userRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResendEmailConfirmationCodeAsync_Should_ReturnFailure_When_EmailAlreadyConfirmed()
    {
        // Arrange
        var request = new ResendEmailConfirmationCodeRequest { Email = "email@gmail.com" };

        User user = new UserFaker().Generate();
        user.EmailConfirmed = true;

        _userRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        Result result = await _authenticationService.ResendEmailConfirmationCodeAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.Conflict);

        await _userRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResendEmailConfirmationCodeAsync_Should_CreateAndReturnSuccess_When_ConfirmationCodeNotExist()
    {
        // Arrange
        var request = new ResendEmailConfirmationCodeRequest { Email = "email@gmail.com" };

        User user = new UserFaker().Generate();
        user.EmailConfirmed = false;

        _userRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(user);

        _emailConfirmationCodeServiceMock
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns((EmailConfirmationCode)null!);

        _emailConfirmationCodeServiceMock
            .CreateEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns("code");

        _eventDispatcherMock
            .DispatchAsync(Arg.Any<ResendEmailConfirmationCodeEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _authenticationService.ResendEmailConfirmationCodeAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _userRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        await _emailConfirmationCodeServiceMock.Received(1)
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());

        await _emailConfirmationCodeServiceMock.Received(1)
            .CreateEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _eventDispatcherMock.Received(1)
            .DispatchAsync(Arg.Any<ResendEmailConfirmationCodeEvent>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResendEmailConfirmationCodeAsync_Should_RefreshAndReturnSuccess_When_ConfirmationCodeExist()
    {
        // Arrange
        var request = new ResendEmailConfirmationCodeRequest { Email = "email@gmail.com" };

        User user = new UserFaker().Generate();
        user.EmailConfirmed = false;

        _userRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(user);

        EmailConfirmationCode emailConfirmationCode = new EmailConfirmationCodeFaker().Generate();

        _emailConfirmationCodeServiceMock
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
            .Returns(emailConfirmationCode);

        _emailConfirmationCodeServiceMock.UpdateEmailConfirmationCode(Arg.Any<EmailConfirmationCode>())
            .Returns("code");

        _eventDispatcherMock
            .DispatchAsync(Arg.Any<ResendEmailConfirmationCodeEvent>(), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>()).
            Returns(1);

        // Act
        Result result = await _authenticationService.ResendEmailConfirmationCodeAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

        await _userRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        await _emailConfirmationCodeServiceMock.Received(1)
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());

        _emailConfirmationCodeServiceMock.Received(1)
            .UpdateEmailConfirmationCode(Arg.Any<EmailConfirmationCode>());

        await _eventDispatcherMock.Received(1)
            .DispatchAsync(Arg.Any<ResendEmailConfirmationCodeEvent>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());
    }
}
