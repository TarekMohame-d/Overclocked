using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Authentication.Commands.ResendEmailConfirmationCode;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Enums;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Shouldly;

namespace Overclocked.Unit.Tests.AuthenticationTests;

public class ResendConfirmationCodeCommandHandlerTest
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ResendEmailConfirmationCodeCommandHandler _resendEmailConfirmationCodeCommandHandler;

    public ResendConfirmationCodeCommandHandlerTest()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _resendEmailConfirmationCodeCommandHandler = new ResendEmailConfirmationCodeCommandHandler(
            _userRepositoryMock,
            _unitOfWorkMock,
            _emailConfirmationCodeServiceMock);
    }

    [Fact]
    public async Task ResendConfirmationCodeCommandHandler_Should_ReturnSuccess_When_EmailNotExist()
    {
        // Arrange
        var command = new ResendEmailConfirmationCodeCommand
        {
            Email = "email@gmail.com"
        };

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User)null!);

        // Act
        Result result = await _resendEmailConfirmationCodeCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResendConfirmationCodeCommandHandler_Should_ReturnFailure_When_EmailAlreadyConfirmed()
    {
        // Arrange
        var command = new ResendEmailConfirmationCodeCommand
        {
            Email = "email@gmail.com"
        };

        User user = new UserFaker(new PasswordHasher()).Generate();
        user.ConfirmEmail();

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);

        // Act
        Result result = await _resendEmailConfirmationCodeCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Conflict);

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResendConfirmationCodeCommandHandler_Should_CreateAndReturnSuccess_When_AllIsValid()
    {
        // Arrange
        var command = new ResendEmailConfirmationCodeCommand
        {
            Email = "email@gmail.com"
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _resendEmailConfirmationCodeCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        user.DomainEvents.ShouldNotBeEmpty();

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
