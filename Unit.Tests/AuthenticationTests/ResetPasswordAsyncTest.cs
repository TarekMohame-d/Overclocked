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

public class ResetPasswordAsyncTest
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

    public ResetPasswordAsyncTest()
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
    public async Task ResetPasswordAsync_When_EmailNotExist_ShouldReturnFailure()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Email = "email@gmail.com",
            Code = "code",
            Password = "password",
        };

        _userRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns((User)null!);

        // Act
        Result result = await _authenticationService.ResetPasswordAsync(request, CancellationToken.None);

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
    public async Task ResetPasswordAsync_When_EmailConfirmationCodeNotExist_ShouldThrowException()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Email = "email@gmail.com",
            Code = "code",
            Password = "password",
        };

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
            await _authenticationService.ResetPasswordAsync(request, CancellationToken.None)
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
    public async Task ResetPasswordAsync_When_EmailConfirmationCodeExpired_ShouldReturnFailure()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Email = "email@gmail.com",
            Code = "code",
            Password = "password",
        };

        User user = new UserFaker().Generate();

        _userRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(user);

        EmailConfirmationCode emailConfirmationCode = new EmailConfirmationCodeFaker().Generate();
        emailConfirmationCode.ExpiredAt = DateTime.UtcNow.AddHours(-1);
        _emailConfirmationCodeServiceMock
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(emailConfirmationCode);

        // Act
        Result result = await _authenticationService.ResetPasswordAsync(request, CancellationToken.None);

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

    [Fact]
    public async Task ResetPasswordAsync_When_EmailConfirmationCodeIsUsed_ShouldReturnFailure()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Email = "email@gmail.com",
            Code = "code",
            Password = "password",
        };

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
        Result result = await _authenticationService.ResetPasswordAsync(request, CancellationToken.None);

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

    [Fact]
    public async Task ResetPasswordAsync_When_CodeIsInvalid_ShouldReturnFailure()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Email = "email@gmail.com",
            Code = "code",
            Password = "password",
        };

        User user = new UserFaker().Generate();

        _userRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(user);

        EmailConfirmationCode emailConfirmationCode = new EmailConfirmationCodeFaker().Generate();
        emailConfirmationCode.IsUsed = false;
        emailConfirmationCode.ExpiredAt = DateTime.UtcNow.AddMinutes(10);
        _emailConfirmationCodeServiceMock
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(emailConfirmationCode);

        _emailConfirmationCodeHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        // Act
        Result result = await _authenticationService.ResetPasswordAsync(request, CancellationToken.None);

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
    public async Task ResetPasswordAsync_When_AllIsValid_ShouldResetPasswordAndReturnSuccess()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Email = "email@gmail.com",
            Code = "code",
            Password = "password",
        };

        User user = new UserFaker().Generate();

        _userRepositoryMock
            .SingleOrDefaultAsync(
                Arg.Any<Expression<Func<User, bool>>>(),
                cancellationToken: Arg.Any<CancellationToken>()
            )
            .Returns(user);

        EmailConfirmationCode emailConfirmationCode = new EmailConfirmationCodeFaker().Generate();
        emailConfirmationCode.IsUsed = false;
        emailConfirmationCode.ExpiredAt = DateTime.UtcNow.AddMinutes(10);
        _emailConfirmationCodeServiceMock
            .GetEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns(emailConfirmationCode);

        _emailConfirmationCodeHasherMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        _passwordHasherMock.Hash(Arg.Any<string>()).Returns("hash");
        _userRepositoryMock.Update(Arg.Any<User>());

        _emailConfirmationCodeServiceMock.InvalidateEmailConfirmationCode(Arg.Any<EmailConfirmationCode>());

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _authenticationService.ResetPasswordAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.OK);

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

        _passwordHasherMock.Received(1).Hash(Arg.Any<string>());

        _userRepositoryMock.Received(1).Update(Arg.Any<User>());

        _emailConfirmationCodeServiceMock.Received(1).InvalidateEmailConfirmationCode(Arg.Any<EmailConfirmationCode>());

        await _unitOfWorkMock.Received(1).CompleteAsync(Arg.Any<CancellationToken>());
    }
}
