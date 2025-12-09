using System.Net;
using NSubstitute;
using Overclocked.Application.Abstraction;
using Overclocked.Application.Abstraction.Persistence;
using Overclocked.Application.Abstraction.Services;
using Overclocked.Application.Authentication.Commands;
using Overclocked.Application.Authentication.Commands.Register;
using Overclocked.Architecture.Tests.FakeData;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate;
using Overclocked.Infrastructure.Authentication;
using Shouldly;

namespace Overclocked.Unit.Tests.AuthenticationTests;

public class RegisterCommandHandlerTest
{
    private readonly IUserRepository _userRepositoryMock;
    private readonly IPermissionRepository _permissionRepositoryMock;
    private readonly ITokenProvider _tokenProviderMock;
    private readonly ITokenReaderService _tokenReaderServiceMock;
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IRefreshTokenHasher _refreshTokenHasherMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly IAuthenticationCommands _authenticationCommands;

    public RegisterCommandHandlerTest()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _permissionRepositoryMock = Substitute.For<IPermissionRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _tokenProviderMock = Substitute.For<ITokenProvider>();
        _refreshTokenHasherMock = Substitute.For<IRefreshTokenHasher>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();
        _tokenReaderServiceMock = Substitute.For<ITokenReaderService>();

        _authenticationCommands = new AuthenticationCommands(
            _userRepositoryMock,
            _permissionRepositoryMock,
            _tokenProviderMock,
            _refreshTokenHasherMock,
            _passwordHasherMock,
            _emailConfirmationCodeServiceMock,
            _tokenReaderServiceMock,
            _unitOfWorkMock);
    }

    [Fact]
    public async Task RegisterCommandHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        var request = new RegisterCommand("email@gmail.com", "password", "first name", "last name", "1234567890");

        User user = new UserFaker(new PasswordHasher()).Generate();

        _userRepositoryMock.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(user);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _authenticationCommands.RegisterCommandHandler(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);
        user.DomainEvents.ShouldNotBeEmpty();
        user.EmailConfirmationCode.ShouldNotBeNull();

        await _userRepositoryMock.Received(1)
            .AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
