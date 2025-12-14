using System.Linq.Expressions;
using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Authentication.Commands.RefreshToken;
using Overclocked.Application.Common.Constants;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Authentication;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.RoleAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Infrastructure.Authentication;
using Shouldly;

namespace Overclocked.Unit.Tests.AuthenticationTests;

public class RefreshTokenCommandHandlerTests
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IPermissionRepository _permissionRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IRefreshTokenHasher _refreshTokenHasherMock;
    private readonly ITokenReaderService _tokenReaderServiceMock;
    private readonly ITokenProvider _tokenProviderMock;
    private readonly RefreshTokenCommandHandler _refreshTokenCommandHandler;

    public RefreshTokenCommandHandlerTests()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _permissionRepositoryMock = Substitute.For<IPermissionRepository>();
        _tokenProviderMock = Substitute.For<ITokenProvider>();
        _refreshTokenHasherMock = Substitute.For<IRefreshTokenHasher>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tokenReaderServiceMock = Substitute.For<ITokenReaderService>();

        _refreshTokenCommandHandler = new RefreshTokenCommandHandler(
            _userRepositoryMock,
            _permissionRepositoryMock,
            _unitOfWorkMock,
            _refreshTokenHasherMock,
            _tokenReaderServiceMock,
            _tokenProviderMock);
    }

    [Fact]
    public async Task RefreshTokenCommandHandler_Should_ReturnFailure_When_AccessTokenIsInvalid()
    {
        // Arrange
        var command = new RefreshTokenCommand
        {
            AccessToken = "invalid-token",
            RefreshToken = "refresh-token"
        };

        _tokenReaderServiceMock.GetUserIdAndDeviceIdFromToken(command.AccessToken)
            .Returns(((Guid, string)?)null);

        // Act
        Result<AuthResponse> result = await _refreshTokenCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        _tokenReaderServiceMock.Received(1)
            .GetUserIdAndDeviceIdFromToken(Arg.Any<string>());
    }

    [Fact]
    public async Task RefreshTokenCommandHandler_Should_ReturnFailure_When_UserIdIsNotGuid()
    {
        // Arrange
        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, "1" },
            { ClaimsConstants.DeviceId, "cf:3d:35:08:e9:df" },
        };

        _tokenReaderServiceMock.ExtractClaimsFromToken(Arg.Any<string>())
            .Returns(claims);

        var command = new RefreshTokenCommand
        {
            AccessToken = "invalid-token",
            RefreshToken = "refresh-token"
        };

        _tokenReaderServiceMock.GetUserIdAndDeviceIdFromToken(Arg.Any<string>())
            .Returns(((Guid, string)?)null);

        // Act
        Result<AuthResponse> result = await _refreshTokenCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        _tokenReaderServiceMock.Received(1)
            .GetUserIdAndDeviceIdFromToken(Arg.Any<string>());
    }

    [Fact]
    public async Task RefreshTokenCommandHandler_Should_ReturnFailure_When_DeviceIdIsNullOrEmpty()
    {
        // Arrange
        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, Guid.NewGuid().ToString() }
        };

        _tokenReaderServiceMock.ExtractClaimsFromToken(Arg.Any<string>())
            .Returns(claims);

        var command = new RefreshTokenCommand
        {
            AccessToken = "invalid-token",
            RefreshToken = "refresh-token"
        };

        _tokenReaderServiceMock.GetUserIdAndDeviceIdFromToken(Arg.Any<string>())
            .Returns(((Guid, string)?)null);

        // Act
        Result<AuthResponse> result = await _refreshTokenCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        _tokenReaderServiceMock.Received(1)
            .GetUserIdAndDeviceIdFromToken(Arg.Any<string>());
    }

    [Fact]
    public async Task RefreshTokenCommandHandler_Should_ReturnFailure_When_UserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, userId.ToString() },
            { ClaimsConstants.DeviceId, "cf:3d:35:08:e9:df" },
        };

        _tokenReaderServiceMock.ExtractClaimsFromToken(Arg.Any<string>())
            .Returns(claims);

        var command = new RefreshTokenCommand
        {
            AccessToken = "invalid-token",
            RefreshToken = "refresh-token"
        };

        _tokenReaderServiceMock.GetUserIdAndDeviceIdFromToken(Arg.Any<string>())
            .Returns((userId, "cf:3d:35:08:e9:df"));

        _userRepositoryMock.GetWithRefreshTokensAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns((User)null!);

        // Act
        Result<AuthResponse> result = await _refreshTokenCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        _tokenReaderServiceMock.Received(1)
            .GetUserIdAndDeviceIdFromToken(Arg.Any<string>());

        await _userRepositoryMock.Received(1)
            .GetWithRefreshTokensAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RefreshTokenCommandHandler_Should_ReturnFailure_When_RefreshTokenIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, userId.ToString() },
            { ClaimsConstants.DeviceId, "cf:3d:35:08:e9:df" },
        };

        _tokenReaderServiceMock.ExtractClaimsFromToken(Arg.Any<string>())
            .Returns(claims);

        var command = new RefreshTokenCommand
        {
            AccessToken = "invalid-token",
            RefreshToken = "refresh-token"
        };

        _tokenReaderServiceMock.GetUserIdAndDeviceIdFromToken(Arg.Any<string>())
            .Returns((userId, "cf:3d:35:08:e9:df"));

        User user = new UserFaker(new PasswordHasher()).Generate();

        user.CreateRefreshToken("cf:3d:35:08:e9:df", "refresh-token-hash");

        _userRepositoryMock.GetWithRefreshTokensAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(user);

        _refreshTokenHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>())
            .Returns(false);

        // Act
        Result<AuthResponse> result = await _refreshTokenCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        _tokenReaderServiceMock.Received(1)
            .GetUserIdAndDeviceIdFromToken(Arg.Any<string>());

        await _userRepositoryMock.Received(1)
            .GetWithRefreshTokensAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        _refreshTokenHasherMock.Received(1)
            .Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task RefreshTokenCommandHandler_Should_ReturnAuthResponse_When_AllIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, userId.ToString() },
            { ClaimsConstants.DeviceId, "cf:3d:35:08:e9:df" },
        };

        _tokenReaderServiceMock.ExtractClaimsFromToken(Arg.Any<string>())
            .Returns(claims);

        var command = new RefreshTokenCommand
        {
            AccessToken = "invalid-token",
            RefreshToken = "refresh-token"
        };

        _tokenReaderServiceMock.GetUserIdAndDeviceIdFromToken(Arg.Any<string>())
            .Returns((userId, "cf:3d:35:08:e9:df"));

        User user = new UserFaker(new PasswordHasher()).Generate();

        user.CreateRefreshToken("cf:3d:35:08:e9:df", "refresh-token-hash");

        _userRepositoryMock.GetWithRefreshTokensAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns(user);

        _refreshTokenHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);

        _permissionRepositoryMock.GetPermissionsByRoleIdAsync(Arg.Any<RoleId>(), Arg.Any<CancellationToken>())
            .Returns([]);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result<AuthResponse> result = await _refreshTokenCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        _tokenReaderServiceMock.Received(1)
            .GetUserIdAndDeviceIdFromToken(Arg.Any<string>());

        await _userRepositoryMock.Received(1)
            .GetWithRefreshTokensAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        _refreshTokenHasherMock.Received(1)
            .Verify(Arg.Any<string>(), Arg.Any<string>());

        await _permissionRepositoryMock.Received(1)
            .GetPermissionsByRoleIdAsync(Arg.Any<RoleId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
