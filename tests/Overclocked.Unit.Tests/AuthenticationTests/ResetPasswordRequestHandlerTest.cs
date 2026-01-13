using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Features.AuthenticationUseCases.ResetPassword;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Overclocked.SharedKernel;
using Shouldly;

namespace Overclocked.Unit.Tests.AuthenticationTests;

public class ResetPasswordRequestHandlerTest
{
    private readonly IAuthenticationRepository _authenticationRepositoryMock;
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly ResetPasswordRequestHandler _resetPasswordRequestHandler;

    public ResetPasswordRequestHandlerTest()
    {
        _authenticationRepositoryMock = Substitute.For<IAuthenticationRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _resetPasswordRequestHandler = new ResetPasswordRequestHandler(
            _authenticationRepositoryMock,
            _unitOfWorkMock,
            _passwordHasherMock,
            _emailConfirmationCodeServiceMock
        );
    }

    [Fact]
    public async Task ResetPasswordRequestHandler_Should_ReturnFailure_When_EmailNotExist()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword123",
            Code = "VF25G4",
        };

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns((User)null!);

        // Act
        Result result = await _resetPasswordRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResetPasswordRequestHandler_Should_ReturnFailure_When_UserIsNotActive()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword123",
            Code = "VF25G4",
        };

        User user = new UserFaker(new PasswordHasher()).Generate();
        user.Deactivate();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        // Act
        Result result = await _resetPasswordRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Forbidden);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResetPasswordRequestHandler_Should_ReturnFailure_When_EmailConfirmationCodeExpiredOrUsed()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword123",
            Code = "VF25G4",
        };

        User user = new UserFaker(new PasswordHasher()).Generate();
        user.ConfirmEmail();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        // Act
        Result result = await _resetPasswordRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResetPasswordRequestHandler_Should_ReturnFailure_When_CodeIsInvalid()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword123",
            Code = "VF25G4",
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        _emailConfirmationCodeServiceMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

        // Act
        Result result = await _resetPasswordRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.BadRequest);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _emailConfirmationCodeServiceMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task ResetPasswordRequestHandler_Should_ReturnFailure_When_NewPasswordIsInvalid()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Email = "email@gmail.com",
            Password = "password",
            Code = "VF25G4",
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        _emailConfirmationCodeServiceMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        // Act
        Result result = await _resetPasswordRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBe(Error.None);
        result.Error.Type.ShouldBe(ErrorType.Validation);

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _emailConfirmationCodeServiceMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public async Task ResetPasswordRequestHandler_Should_ResetPasswordAndReturnSuccess_When_AllIsValid()
    {
        // Arrange
        var request = new ResetPasswordRequest
        {
            Email = "email@gmail.com",
            Password = "P@ssword123",
            Code = "VF25G4",
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _authenticationRepositoryMock.GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(user);

        _emailConfirmationCodeServiceMock.Verify(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>()).Returns(1);

        // Act
        Result result = await _resetPasswordRequestHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();

        await _authenticationRepositoryMock.Received(1).GetByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());

        _emailConfirmationCodeServiceMock.Received(1).Verify(Arg.Any<string>(), Arg.Any<string>());

        await _unitOfWorkMock.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
