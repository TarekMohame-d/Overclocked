using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants;
using Overclocked.Application.Features.AuthenticationUseCases.DTOs.Responses;
using Overclocked.Application.Features.AuthenticationUseCases.RefreshToken;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.Domain.UserAggregate.ValueObjects;
using Overclocked.Infrastructure.Authentication;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.AuthenticationTests;

public class RefreshTokenRequestHandlerTests
{
    private readonly IAuthenticationRepository _authenticationRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IRefreshTokenHasher _refreshTokenHasherMock;
    private readonly ITokenReaderService _tokenReaderServiceMock;
    private readonly ITokenProvider _tokenProviderMock;
    private readonly RefreshTokenRequestHandler _refreshTokenRequestHandler;

    public RefreshTokenRequestHandlerTests()
    {
        _authenticationRepositoryMock = Substitute.For<IAuthenticationRepository>();
        _tokenProviderMock = Substitute.For<ITokenProvider>();
        _refreshTokenHasherMock = Substitute.For<IRefreshTokenHasher>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tokenReaderServiceMock = Substitute.For<ITokenReaderService>();

        _refreshTokenRequestHandler = new RefreshTokenRequestHandler(
            _authenticationRepositoryMock,
            _unitOfWorkMock,
            _refreshTokenHasherMock,
            _tokenReaderServiceMock,
            _tokenProviderMock
        );
    }

    [Fact]
    public async Task RefreshTokenRequestHandler_Should_ReturnFailure_When_AccessTokenIsInvalid()
    {
        // Arrange
        var request = new RefreshTokenRequest { AccessToken = "invalid-token", RefreshToken = "refresh-token" };

        // Act
        Result<AuthResponse> result = await _refreshTokenRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);
    }

    [Fact]
    public async Task RefreshTokenRequestHandler_Should_ReturnFailure_When_UserIdIsNotGuid()
    {
        // Arrange
        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, "1" },
            { ClaimsConstants.DeviceId, Guid.NewGuid().ToString() },
        };

        _tokenReaderServiceMock.ExtractClaimsFromToken(Arg.Any<string>()).Returns(claims);

        var request = new RefreshTokenRequest { AccessToken = "invalid-token", RefreshToken = "refresh-token" };

        // Act
        Result<AuthResponse> result = await _refreshTokenRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);
    }

    [Fact]
    public async Task RefreshTokenRequestHandler_Should_ReturnFailure_When_DeviceIdIsNotGuid()
    {
        // Arrange
        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, Guid.NewGuid().ToString() },
            { ClaimsConstants.DeviceId, "1" },
        };

        _tokenReaderServiceMock.ExtractClaimsFromToken(Arg.Any<string>()).Returns(claims);

        var request = new RefreshTokenRequest { AccessToken = "invalid-token", RefreshToken = "refresh-token" };

        // Act
        Result<AuthResponse> result = await _refreshTokenRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);
    }

    [Fact]
    public async Task RefreshTokenRequestHandler_Should_ReturnFailure_When_DeviceIdIsNullOrEmpty()
    {
        // Arrange
        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, Guid.NewGuid().ToString() },
        };

        _tokenReaderServiceMock.ExtractClaimsFromToken(Arg.Any<string>()).Returns(claims);

        var request = new RefreshTokenRequest { AccessToken = "invalid-token", RefreshToken = "refresh-token" };

        // Act
        Result<AuthResponse> result = await _refreshTokenRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);
    }

    [Fact]
    public async Task RefreshTokenRequestHandler_Should_ReturnFailure_When_UserNotFound()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, userId.ToString() },
            { ClaimsConstants.DeviceId, deviceId.ToString() },
        };

        _tokenReaderServiceMock.ExtractClaimsFromToken(Arg.Any<string>()).Returns(claims);

        var request = new RefreshTokenRequest { AccessToken = "invalid-token", RefreshToken = "refresh-token" };

        _authenticationRepositoryMock
            .GetWithRefreshTokensAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>())
            .Returns((User)null!);

        // Act
        Result<AuthResponse> result = await _refreshTokenRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);
    }

    [Fact]
    public async Task RefreshTokenRequestHandler_Should_ReturnFailure_When_UserIsNotActive()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, userId.ToString() },
            { ClaimsConstants.DeviceId, deviceId.ToString() },
        };

        _tokenReaderServiceMock.ExtractClaimsFromToken(Arg.Any<string>()).Returns(claims);

        var request = new RefreshTokenRequest { AccessToken = "invalid-token", RefreshToken = "refresh-token" };

        User user = new UserFaker(new PasswordHasher()).Generate();

        user.CreateRefreshToken(deviceId, "refresh-token-hash");

        user.Deactivate();

        _authenticationRepositoryMock.GetWithRefreshTokensAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(user);

        // Act
        Result<AuthResponse> result = await _refreshTokenRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
    }

    [Fact]
    public async Task RefreshTokenRequestHandler_Should_ReturnFailure_When_RefreshTokenIsInvalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, userId.ToString() },
            { ClaimsConstants.DeviceId, deviceId.ToString() },
        };

        _tokenReaderServiceMock.ExtractClaimsFromToken(Arg.Any<string>()).Returns(claims);

        _tokenReaderServiceMock.GetUserIdAndDeviceIdFromToken(Arg.Any<string>()).Returns((userId, deviceId));

        var request = new RefreshTokenRequest { AccessToken = "invalid-token", RefreshToken = "refresh-token" };

        User user = new UserFaker(new PasswordHasher()).Generate();

        user.CreateRefreshToken(deviceId, "refresh-token-hash");

        _authenticationRepositoryMock.GetWithRefreshTokensAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(user);

        _refreshTokenHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        // Act
        Result<AuthResponse> result = await _refreshTokenRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _authenticationRepositoryMock
            .Received(1)
            .GetWithRefreshTokensAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        _refreshTokenHasherMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task RefreshTokenRequestHandler_Should_ReturnAuthResponse_When_AllIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var deviceId = Guid.NewGuid();
        IDictionary<string, string> claims = new Dictionary<string, string>
        {
            { ClaimsConstants.NameIdentifier, userId.ToString() },
            { ClaimsConstants.DeviceId, deviceId.ToString() },
        };

        _tokenReaderServiceMock.ExtractClaimsFromToken(Arg.Any<string>()).Returns(claims);

        _tokenReaderServiceMock.GetUserIdAndDeviceIdFromToken(Arg.Any<string>()).Returns((userId, deviceId));

        _tokenProviderMock.GenerateRefreshToken().Returns("refresh-token");

        _refreshTokenHasherMock.Hash(Arg.Any<string>()).Returns("refresh-token-hash");

        var request = new RefreshTokenRequest { AccessToken = "invalid-token", RefreshToken = "refresh-token" };

        User user = new UserFaker(new PasswordHasher()).Generate();

        user.CreateRefreshToken(deviceId, "refresh-token-hash");

        _authenticationRepositoryMock.GetWithRefreshTokensAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>()).Returns(user);

        _refreshTokenHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        _authenticationRepositoryMock.GetPermissionsAsync(Arg.Any<Role>(), Arg.Any<CancellationToken>()).Returns([]);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result<AuthResponse> result = await _refreshTokenRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _authenticationRepositoryMock
            .Received(1)
            .GetWithRefreshTokensAsync(Arg.Any<UserId>(), Arg.Any<CancellationToken>());

        _refreshTokenHasherMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());

        await _authenticationRepositoryMock.Received(1).GetPermissionsAsync(Arg.Any<Role>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
