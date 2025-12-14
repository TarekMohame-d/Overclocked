using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Authentication.Commands.Login;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Contracts.Authentication;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.RoleAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Shouldly;

namespace Overclocked.Unit.Tests.AuthenticationTests;

public class LoginCommandHandlerTest
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IPermissionRepository _permissionRepositoryMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly IRefreshTokenHasher _refreshTokenHasherMock;
    private readonly ITokenProvider _tokenProviderMock;
    private readonly LoginCommandHandler _loginCommandHandler;

    public LoginCommandHandlerTest()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _permissionRepositoryMock = Substitute.For<IPermissionRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _tokenProviderMock = Substitute.For<ITokenProvider>();
        _refreshTokenHasherMock = Substitute.For<IRefreshTokenHasher>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _loginCommandHandler = new LoginCommandHandler(
            _userRepositoryMock,
            _permissionRepositoryMock,
            _unitOfWorkMock,
            _emailConfirmationCodeServiceMock,
            _passwordHasherMock,
            _refreshTokenHasherMock,
            _tokenProviderMock);
    }

    [Fact]
    public async Task LoginCommandHandler_Should_ReturnFailure_When_UserWithEmailNotExist()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "email@gmail.com",
            Password = "password",
            DeviceId = "device-id"
        };

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User)null!);

        _passwordHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>())
            .Returns(false);

        // Act
        Result result = await _loginCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _passwordHasherMock.Received(1)
            .Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task LoginCommandHandler_Should_ReturnFailure_When_PasswordIsInvalid()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "email@gmail.com",
            Password = "password",
            DeviceId = "device-id"
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);

        _passwordHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>())
            .Returns(false);

        // Act
        Result<AuthResponse> result = await _loginCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _passwordHasherMock.Received(1)
            .Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task LoginCommandHandler_Should_ReturnFailure_When_EmailIsNotConfirmed()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "email@gmail.com",
            Password = "password",
            DeviceId = "device-id"
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);

        _passwordHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);

        // Act
        Result<AuthResponse> result = await _loginCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);
        user.DomainEvents.ShouldNotBeEmpty();

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _passwordHasherMock.Received(1)
            .Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task LoginCommandHandler_Should_ReturnAuthResponse_When_AllDataValid()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "email@gmail.com",
            Password = "password",
            DeviceId = "device-id"
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        user.ConfirmEmail();

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);

        _passwordHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);

        _permissionRepositoryMock.GetPermissionsByRoleIdAsync(Arg.Any<RoleId>(), Arg.Any<CancellationToken>())
            .Returns([]);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result<AuthResponse> result = await _loginCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _passwordHasherMock.Received(1)
            .Verify(Arg.Any<string>(), Arg.Any<string>());

        await _permissionRepositoryMock.Received(1)
            .GetPermissionsByRoleIdAsync(Arg.Any<RoleId>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
