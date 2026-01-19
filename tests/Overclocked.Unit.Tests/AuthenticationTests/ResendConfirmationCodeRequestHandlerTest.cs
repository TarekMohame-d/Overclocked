using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Features.AuthenticationUseCases.ResendEmailConfirmationCode;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.AuthenticationTests;

public class ResendConfirmationCodeRequestHandlerTest
{
    private readonly IAuthenticationRepository _authenticationRepositoryMock;
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ResendEmailConfirmationCodeRequestHandler _resendEmailConfirmationCodeRequestHandler;

    public ResendConfirmationCodeRequestHandlerTest()
    {
        _authenticationRepositoryMock = Substitute.For<IAuthenticationRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _resendEmailConfirmationCodeRequestHandler = new ResendEmailConfirmationCodeRequestHandler(
            _authenticationRepositoryMock,
            _unitOfWorkMock,
            _emailConfirmationCodeServiceMock
        );
    }

    [Fact]
    public async Task ResendConfirmationCodeRequestHandler_Should_ReturnSuccess_When_EmailNotExist()
    {
        // Arrange
        var request = new ResendEmailConfirmationCodeRequest { Email = "email@gmail.com" };

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User)null!);

        // Act
        Result result = await _resendEmailConfirmationCodeRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResendConfirmationCodeRequestHandler_Should_ReturnFailure_When_UserIsNotActive()
    {
        // Arrange
        var request = new ResendEmailConfirmationCodeRequest { Email = "email@gmail.com" };

        User user = new UserFaker(new PasswordHasher()).Generate();
        user.Deactivate();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        // Act
        Result result = await _resendEmailConfirmationCodeRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Forbidden);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResendConfirmationCodeRequestHandler_Should_ReturnFailure_When_EmailAlreadyConfirmed()
    {
        // Arrange
        var request = new ResendEmailConfirmationCodeRequest { Email = "email@gmail.com" };

        User user = new UserFaker(new PasswordHasher()).Generate();
        user.ConfirmEmail();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        // Act
        Result result = await _resendEmailConfirmationCodeRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResendConfirmationCodeRequestHandler_Should_CreateAndReturnSuccess_When_AllIsValid()
    {
        // Arrange
        var request = new ResendEmailConfirmationCodeRequest { Email = "email@gmail.com" };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _resendEmailConfirmationCodeRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        user.DomainEvents.ShouldNotBeEmpty();

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
