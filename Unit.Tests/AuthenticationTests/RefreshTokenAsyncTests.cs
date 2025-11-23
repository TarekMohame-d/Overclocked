using System.Linq.Expressions;
using System.Net;
using Application.Abstraction.DomainServices;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Constants;
using Application.Common.Results;
using Application.Services.Authentication;
using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.Helpers.Interfaces;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.AuthenticationTests;

public class RefreshTokenAsyncTests
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

    public RefreshTokenAsyncTests()
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
    public async Task RefreshTokenAsync_Should_ReturnFailure_When_AccessTokenIsInvalid()
    {
        // Arrange
        var request = new RefreshTokenRequest { AccessToken = "access-token", RefreshToken = "refresh-token" };

        _tokenReaderServiceMock.GetClaimsFromToken(Arg.Any<string>())
            .Returns((IDictionary<string, string>)null!);

        // Act
        Result result = await _authenticationService.RefreshTokenAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        _tokenReaderServiceMock.Received(1)
            .GetClaimsFromToken(Arg.Any<string>());
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_ReturnFailure_When_UserIdIsNotGuid()
    {
        // Arrange
        var request = new RefreshTokenRequest { AccessToken = "access-token", RefreshToken = "refresh-token" };

        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, "1" },
            { ClaimsConstants.DeviceId, "cf:3d:35:08:e9:df" },
        };

        _tokenReaderServiceMock.GetClaimsFromToken(Arg.Any<string>()).Returns(claims);

        // Act
        Result result = await _authenticationService.RefreshTokenAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        _tokenReaderServiceMock.Received(1)
            .GetClaimsFromToken(Arg.Any<string>());
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_ReturnFailure_When_DeviceIdIsNullOrEmpty()
    {
        // Arrange
        var request = new RefreshTokenRequest { AccessToken = "access-token", RefreshToken = "refresh-token" };

        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, Guid.NewGuid().ToString() },
        };

        _tokenReaderServiceMock.GetClaimsFromToken(Arg.Any<string>())
            .Returns(claims);

        // Act
        Result result = await _authenticationService.RefreshTokenAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        _tokenReaderServiceMock.Received(1)
            .GetClaimsFromToken(Arg.Any<string>());
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_ReturnFailure_When_UserNotFound()
    {
        // Arrange
        var request = new RefreshTokenRequest { AccessToken = "access-token", RefreshToken = "refresh-token" };

        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, Guid.NewGuid().ToString() },
            { ClaimsConstants.DeviceId, "cf:3d:35:08:e9:df" },
        };

        _tokenReaderServiceMock.GetClaimsFromToken(Arg.Any<string>()).Returns(claims);

        _userRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns((User)null!);

        // Act
        Result result = await _authenticationService.RefreshTokenAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        _tokenReaderServiceMock.Received(1)
            .GetClaimsFromToken(Arg.Any<string>());

        await _userRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshTokenAsync_Should_ReturnFailure_When_RefreshTokenIsInvalid()
    {
        // Arrange
        var request = new RefreshTokenRequest { AccessToken = "access-token", RefreshToken = "refresh-token" };

        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, Guid.NewGuid().ToString() },
            { ClaimsConstants.DeviceId, "cf:3d:35:08:e9:df" },
        };

        _tokenReaderServiceMock.GetClaimsFromToken(Arg.Any<string>())
            .Returns(claims);

        _userRepositoryMock.SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>())
            .Returns(new UserFaker().Generate());

        _refreshTokenServiceMock.VerifyRefreshTokenAsync(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            request.RefreshToken,
            Arg.Any<CancellationToken>())
            .Returns(false);

        // Act
        Result result = await _authenticationService.RefreshTokenAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        _tokenReaderServiceMock.Received(1)
            .GetClaimsFromToken(Arg.Any<string>());

        await _userRepositoryMock.Received(1)
            .SingleOrDefaultAsync(
            Arg.Any<Expression<Func<User, bool>>>(),
            cancellationToken: Arg.Any<CancellationToken>());

        await _refreshTokenServiceMock.Received(1)
            .VerifyRefreshTokenAsync(
            Arg.Any<Guid>(),
            Arg.Any<string>(),
            request.RefreshToken,
            Arg.Any<CancellationToken>());
    }
}
