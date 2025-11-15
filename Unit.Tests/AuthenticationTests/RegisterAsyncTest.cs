using System.Net;
using Application.Abstraction.Messaging;
using Application.Abstraction.Repositories;
using Application.Abstraction.Services;
using Application.Common.Results;
using Application.Services.Authentication;
using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.Events;
using Application.Services.Authentication.Helpers.Interfaces;
using ArchitectureTests.FakeData;
using Domain.Entities;
using NSubstitute;
using Shouldly;

namespace Unit.Tests.AuthenticationTests;

public class RegisterAsyncTest
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

    public RegisterAsyncTest()
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

        _authenticationService = new AuthenticationService(_userRepositoryMock, _rolePermissionsRepositoryMock, _unitOfWorkMock, _passwordHasherMock,
            _eventDispatcherMock, _emailConfirmationCodeHasherMock, _emailConfirmationCodeServiceMock,
            _tokenProviderMock, _refreshTokenServiceMock, _tokenReaderServiceMock);
    }

    [Fact]
    public async Task RegisterAsync_When_ThereIsNoError_ShouldReturnSuccess()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "email@gmail.com",
            Password = "password",
            PhoneNumber = "1234567890",
            FirstName = "first name",
            LastName = "last name"
        };

        User user = new UserFaker().Generate();

        _userRepositoryMock.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(user);

        _emailConfirmationCodeServiceMock
            .CreateEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns("code");

        _eventDispatcherMock.DispatchAsync(Arg.Any<UserRegisteredEvent>())
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.CompleteAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _authenticationService.RegisterAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.StatusCode.ShouldBe(HttpStatusCode.Created);

        await _userRepositoryMock.Received(1)
            .AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .CompleteAsync(Arg.Any<CancellationToken>());

        await _eventDispatcherMock.Received(1)
            .DispatchAsync(Arg.Any<UserRegisteredEvent>());

        await _emailConfirmationCodeServiceMock.Received(1)
            .CreateEmailConfirmationCodeAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
