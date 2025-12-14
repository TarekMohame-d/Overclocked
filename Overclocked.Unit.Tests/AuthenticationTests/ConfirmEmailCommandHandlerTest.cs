using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
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
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ConfirmEmailCommandHandler _confirmEmailCommandHandler;

    public ConfirmEmailCommandHandlerTest()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _confirmEmailCommandHandler = new ConfirmEmailCommandHandler(
            _userRepositoryMock,
            _unitOfWorkMock,
            _emailConfirmationCodeServiceMock);
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
        Result result = await _confirmEmailCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
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
        Result result = await _confirmEmailCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
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
        Result result = await _confirmEmailCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
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
        Result result = await _confirmEmailCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _emailConfirmationCodeServiceMock.Received(1)
            .Verify(Arg.Any<string>(), Arg.Any<string>());
    }
}
