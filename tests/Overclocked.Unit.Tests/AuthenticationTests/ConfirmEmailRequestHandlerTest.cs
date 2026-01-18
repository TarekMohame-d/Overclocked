using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Features.AuthenticationUseCases.ConfirmEmail;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.AuthenticationTests;

public class ConfirmEmailRequestHandlerTest
{
    private readonly IAuthenticationRepository _authenticationRepositoryMock;
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ConfirmEmailRequestHandler _confirmEmailRequestHandler;

    public ConfirmEmailRequestHandlerTest()
    {
        _authenticationRepositoryMock = Substitute.For<IAuthenticationRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _confirmEmailRequestHandler = new ConfirmEmailRequestHandler(
            _authenticationRepositoryMock,
            _unitOfWorkMock,
            _emailConfirmationCodeServiceMock
        );
    }

    [Fact]
    public async Task ConfirmEmailRequestHandler_Should_ReturnFailure_When_EmailNotExist()
    {
        // Arrange
        var request = new ConfirmEmailRequest { Email = "email@gmail.com", Code = "VF25G4" };

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User)null!);

        // Act
        Result result = await _confirmEmailRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmEmailRequestHandler_Should_ReturnFailure_When_EmailIsAlreadyConfirmed()
    {
        // Arrange
        var request = new ConfirmEmailRequest { Email = "email@gmail.com", Code = "VF25G4" };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        user.ConfirmEmail();

        // Act
        Result result = await _confirmEmailRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmEmailRequestHandler_Should_ReturnFailure_When_ConfirmationCodeIsInvalid()
    {
        // Arrange
        var request = new ConfirmEmailRequest { Email = "email@gmail.com", Code = "VF25G4" };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        _emailConfirmationCodeServiceMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        // Act
        Result result = await _confirmEmailRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmEmailRequestHandler_Should_ReturnSuccess_When_AllIsGood()
    {
        // Arrange
        var request = new ConfirmEmailRequest { Email = "email@gmail.com", Code = "VF25G4" };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        _emailConfirmationCodeServiceMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _confirmEmailRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBe(Error.None);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _emailConfirmationCodeServiceMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());
    }
}
