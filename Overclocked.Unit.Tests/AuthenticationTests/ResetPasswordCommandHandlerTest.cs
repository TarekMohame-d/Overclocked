using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Authentication.Commands.ResetPassword;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Shouldly;

namespace Overclocked.Unit.Tests.AuthenticationTests;

public class ResetPasswordCommandHandlerTest
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ResetPasswordCommandHandler _resetPasswordCommandHandler;

    public ResetPasswordCommandHandlerTest()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _resetPasswordCommandHandler = new ResetPasswordCommandHandler(
            _userRepositoryMock,
            _unitOfWorkMock,
            _passwordHasherMock,
            _emailConfirmationCodeServiceMock);
    }

    [Fact]
    public async Task ResetPasswordCommandHandler_Should_ReturnFailure_When_EmailNotExist()
    {
        // Arrange
        var command = new ResetPasswordCommand
        {
            Email = "email@gmail.com",
            Password = "password",
            Code = "VF25G4"
        };

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User)null!);

        // Act
        Result result = await _resetPasswordCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResetPasswordCommandHandler_Should_ReturnFailure_When_EmailConfirmationCodeExpiredOrUsed()
    {
        // Arrange
        var command = new ResetPasswordCommand
        {
            Email = "email@gmail.com",
            Password = "password",
            Code = "VF25G4"
        };

        User user = new UserFaker(new PasswordHasher()).Generate();
        user.ConfirmEmail();

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        Result result = await _resetPasswordCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResetPasswordCommandHandler_Should_ReturnFailure_When_CodeIsInvalid()
    {
        // Arrange
        var command = new ResetPasswordCommand
        {
            Email = "email@gmail.com",
            Password = "password",
            Code = "VF25G4"
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);

        _emailConfirmationCodeServiceMock
            .Verify(Arg.Any<string>(), Arg.Any<string>())
            .Returns(false);

        // Act
        Result result = await _resetPasswordCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _emailConfirmationCodeServiceMock.Received(1)
            .Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task ResetPasswordCommandHandler_Should_ResetPasswordAndReturnSuccess_When_AllIsValid()
    {
        // Arrange
        var command = new ResetPasswordCommand
        {
            Email = "email@gmail.com",
            Password = "password",
            Code = "VF25G4"
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);

        _emailConfirmationCodeServiceMock
            .Verify(Arg.Any<string>(), Arg.Any<string>())
            .Returns(true);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _resetPasswordCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _emailConfirmationCodeServiceMock.Received(1)
            .Verify(Arg.Any<string>(), Arg.Any<string>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
