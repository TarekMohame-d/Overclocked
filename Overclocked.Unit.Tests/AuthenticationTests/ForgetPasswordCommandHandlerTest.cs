using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Authentication.Commands.ForgetPassword;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Shouldly;

namespace Overclocked.Unit.Tests.AuthenticationTests;

public class ForgetPasswordCommandHandlerTest
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ForgetPasswordCommandHandler _forgetPasswordCommandHandler;

    public ForgetPasswordCommandHandlerTest()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _forgetPasswordCommandHandler = new ForgetPasswordCommandHandler(
            _userRepositoryMock,
            _unitOfWorkMock,
            _emailConfirmationCodeServiceMock);
    }

    [Fact]
    public async Task ForgetPasswordCommandHandler_Should_ReturnSuccess_When_UserNotExist()
    {
        // Arrange
        var command = new ForgetPasswordCommand
        {
            Email = "email@gmail.com"
        };

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((User)null!);

        // Act
        Result result = await _forgetPasswordCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ForgetPasswordCommandHandler_Should_ReturnSuccess_When_AllDataValid()
    {
        // Arrange
        var command = new ForgetPasswordCommand
        {
            Email = "email@gmail.com"
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _userRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(user);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _forgetPasswordCommandHandler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        user.DomainEvents.ShouldNotBeEmpty();

        await _userRepositoryMock.Received(1)
            .GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
