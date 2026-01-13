using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Features.AuthenticationUseCases.ForgetPassword;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.AuthenticationTests;

public class ForgetPasswordRequestHandlerTest
{
    private readonly IAuthenticationRepository _authenticationRepositoryMock;
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ForgetPasswordRequestHandler _forgetPasswordRequestHandler;

    public ForgetPasswordRequestHandlerTest()
    {
        _authenticationRepositoryMock = Substitute.For<IAuthenticationRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _forgetPasswordRequestHandler = new ForgetPasswordRequestHandler(
            _authenticationRepositoryMock,
            _unitOfWorkMock,
            _emailConfirmationCodeServiceMock
        );
    }

    [Fact]
    public async Task ForgetPasswordRequestHandler_Should_ReturnSuccess_When_UserNotExist()
    {
        // Arrange
        var request = new ForgetPasswordRequest { Email = "email@gmail.com" };

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User)null!);

        // Act
        Result result = await _forgetPasswordRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ForgetPasswordRequestHandler_Should_ReturnFailure_When_UserIsNotActive()
    {
        // Arrange
        var request = new ForgetPasswordRequest { Email = "email@gmail.com" };

        User user = new UserFaker(new PasswordHasher()).Generate();

        user.Deactivate();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        // Act
        Result result = await _forgetPasswordRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ForgetPasswordRequestHandler_Should_ReturnSuccess_When_AllDataValid()
    {
        // Arrange
        var request = new ForgetPasswordRequest { Email = "email@gmail.com" };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _forgetPasswordRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);
        user.DomainEvents.ShouldNotBeEmpty();

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
