using System.Linq.Expressions;
using System.Net;
using Application.Abstraction.DomainServices;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Services.Authentication;
using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.Helpers.Interfaces;
using ArchitectureTests.FakeData;
using Domain.Entities;
using Domain.Exceptions;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.AuthenticationTests;

public class ConfirmEmailAsyncTest
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

    public ConfirmEmailAsyncTest()
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
    public async Task ConfirmEmailAsync_When_EmailNotExist_ShouldReturnFailure()
    {
        // Arrange
        var request = new ConfirmEmailRequest { Email = "email@gmail.com", Code = "VF25G4" };

        _userRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns((User)null!);

        // Act
        Result result = await _authenticationService.ConfirmEmailAsync(request, CancellationToken.None);

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
    }

    [Fact]
    public async Task ConfirmEmailAsync_When_EmailConfirmationCodeNotExist_ShouldThrowException()
    {
        // Arrange
        var request = new ConfirmEmailRequest { Email = "email@gmail.com", Code = "VF25G4" };

        User user = new UserFaker().Generate();

        _userRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(user);

        _emailConfirmationCodeServiceMock
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((EmailConfirmationCode)null!);

        // Act
        Exception exception = await Should.ThrowAsync<Exception>(async () =>
            await _authenticationService.ConfirmEmailAsync(request, CancellationToken.None)
        );

        // Assert
        exception.ShouldBeOfType<EmailConfirmationCodeNotExistException>();

        await _userRepositoryMock
            .Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            );

        await _emailConfirmationCodeServiceMock
            .Received(1)
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmEmailAsync_When_EmailIsAlreadyConfirmed_ShouldReturnFailure()
    {
        // Arrange
        var request = new ConfirmEmailRequest { Email = "email@gmail.com", Code = "VF25G4" };

        User user = new UserFaker().Generate();

        _userRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(user);

        EmailConfirmationCode emailConfirmationCode = new EmailConfirmationCodeFaker().Generate();
        user.EmailConfirmed = true;

        _emailConfirmationCodeServiceMock
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(emailConfirmationCode);

        // Act
        Result result = await _authenticationService.ConfirmEmailAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.Conflict);

        await _userRepositoryMock
            .Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            );

        await _emailConfirmationCodeServiceMock
            .Received(1)
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmEmailAsync_When_EmailConfirmationCodeIsUsed_ShouldReturnFailure()
    {
        // Arrange
        var request = new ConfirmEmailRequest { Email = "email@gmail.com", Code = "VF25G4" };

        User user = new UserFaker().Generate();

        _userRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(user);

        EmailConfirmationCode emailConfirmationCode = new EmailConfirmationCodeFaker().Generate();
        emailConfirmationCode.IsUsed = true;

        _emailConfirmationCodeServiceMock
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(emailConfirmationCode);

        // Act
        Result result = await _authenticationService.ConfirmEmailAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        result.Error.ShouldNotBeNull();
        result.Error.Type.ShouldBe(ErrorType.Conflict);

        await _userRepositoryMock
            .Received(1)
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            );

        await _emailConfirmationCodeServiceMock
            .Received(1)
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmEmailAsync_When_ConfirmationCodeIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        var request = new ConfirmEmailRequest { Email = "email@gmail.com", Code = "VF25G4" };

        User user = new UserFaker().Generate();
        user.EmailConfirmed = false;
        _userRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(user);

        EmailConfirmationCode emailConfirmationCode = new EmailConfirmationCodeFaker().Generate();

        _emailConfirmationCodeServiceMock
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(emailConfirmationCode);

        emailConfirmationCode.ExpiredAt = DateTime.UtcNow.AddMinutes(10);
        emailConfirmationCode.IsUsed = false;

        _emailConfirmationCodeHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        // Act
        Result result = await _authenticationService.ConfirmEmailAsync(request, CancellationToken.None);

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

        await _emailConfirmationCodeServiceMock
            .Received(1)
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        _emailConfirmationCodeHasherMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task ConfirmEmailAsync_When_ConfirmationCodeExpired_ShouldReturnFailure()
    {
        // Arrange
        var request = new ConfirmEmailRequest { Email = "email@gmail.com", Code = "VF25G4" };

        User user = new UserFaker().Generate();
        user.EmailConfirmed = false;
        _userRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(user);

        EmailConfirmationCode emailConfirmationCode = new EmailConfirmationCodeFaker().Generate();
        _emailConfirmationCodeServiceMock
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(emailConfirmationCode);

        emailConfirmationCode.ExpiredAt = DateTime.UtcNow.AddHours(-1);
        emailConfirmationCode.IsUsed = false;

        // Act
        Result result = await _authenticationService.ConfirmEmailAsync(request, CancellationToken.None);

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

        await _emailConfirmationCodeServiceMock
            .Received(1)
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
