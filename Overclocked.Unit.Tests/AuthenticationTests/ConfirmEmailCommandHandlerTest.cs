using System.Linq.Expressions;
using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Application.Authentication.Commands;
using Overclocked.Application.Authentication.Commands.ConfirmEmail;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Shouldly;

namespace Overclocked.Unit.Tests.AuthenticationTests;

public class ConfirmEmailCommandHandlerTest
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IPermissionRepository _permissionRepositoryMock;
    private readonly ITokenProvider _tokenProviderMock;
    private readonly ITokenReaderService _tokenReaderServiceMock;
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IRefreshTokenHasher _refreshTokenHasherMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IAuthenticationCommands _authenticationCommands;

    public ConfirmEmailCommandHandlerTest()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _permissionRepositoryMock = Substitute.For<IPermissionRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _tokenProviderMock = Substitute.For<ITokenProvider>();
        _refreshTokenHasherMock = Substitute.For<IRefreshTokenHasher>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tokenReaderServiceMock = Substitute.For<ITokenReaderService>();

        _authenticationCommands = new AuthenticationCommands(
            _userRepositoryMock,
            _permissionRepositoryMock,
            _tokenProviderMock,
            _refreshTokenHasherMock,
            _passwordHasherMock,
            _emailConfirmationCodeServiceMock,
            _tokenReaderServiceMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task ConfirmEmailCommandHandler_Should_ReturnFailure_When_EmailNotExist()
    {
        // Arrange
        var command = new ConfirmEmailCommand
        {
            Email = "email@gmail.com",
            Code = "VF25G4"
        };

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User)null!);

        // Act
        Result result = await _authenticationCommands.ConfirmEmailCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmEmailCommandHandler_Should_ReturnFailure_When_EmailIsAlreadyConfirmed()
    {
        // Arrange
        var command = new ConfirmEmailCommand
        {
            Email = "email@gmail.com",
            Code = "VF25G4"
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);

        user.ConfirmEmail();

        // Act
        Result result = await _authenticationCommands.ConfirmEmailCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Conflict);

        await _userRepositoryMock.Received(1).
            GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmEmailCommandHandler_Should_ReturnFailure_When_ConfirmationCodeIsInvalid()
    {
        // Arrange
        var command = new ConfirmEmailCommand
        {
            Email = "email@gmail.com",
            Code = "VF25G4"
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);

        _emailConfirmationCodeServiceMock.Verify(Arg.Any<string>(), Arg.Any<string>())
            .Returns(false);

        // Act
        Result result = await _authenticationCommands.ConfirmEmailCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmEmailCommandHandler_Should_ReturnSuccess_When_AllIsGood()
    {
        // Arrange
        var command = new ConfirmEmailCommand
        {
            Email = "email@gmail.com",
            Code = "VF25G4"
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);

        _emailConfirmationCodeServiceMock.Verify(Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _authenticationCommands.ConfirmEmailCommandHandler(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);
        result.Error.ShouldBe(Error.None);

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _emailConfirmationCodeServiceMock.Received(1)
            .Verify(Arg.Any<string>(), Arg.Any<string>());
    }
}
