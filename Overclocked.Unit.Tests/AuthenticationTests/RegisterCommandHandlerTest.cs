using NSubstitute;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
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
    private readonly IEmailConfirmationCodeService _emailConfirmationCodeServiceMock;
    private readonly IPasswordHasher _passwordHasherMock;
    private readonly IUnitOfWork _unitOfWorkMock;
    private readonly RegisterCommandHandler _registerCommandHandler;

    public RegisterCommandHandlerTest()
    {
        _userRepositoryMock = Substitute.For<IUserRepository>();
        _emailConfirmationCodeServiceMock = Substitute.For<IEmailConfirmationCodeService>();
        _passwordHasherMock = Substitute.For<IPasswordHasher>();
        _unitOfWorkMock = Substitute.For<IUnitOfWork>();

        _registerCommandHandler = new RegisterCommandHandler(
            _userRepositoryMock,
            _unitOfWorkMock,
            _passwordHasherMock,
            _emailConfirmationCodeServiceMock);
    }

    [Fact]
    public async Task RegisterCommandHandler_Should_ReturnSuccess_When_ThereIsNoError()
    {
        // Arrange
        var request = new RegisterCommand
        {
            Email = "email@gmail.com",
            Password = "password",
            FirstName = "first name",
            LastName = "last name",
            PhoneNumber = "1234567890"
        };

        User user = new UserFaker(new PasswordHasher()).Generate();

        _userRepositoryMock.AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(user);

        _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(1);

        // Act
        Result result = await _registerCommandHandler.Handle(request, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        user.DomainEvents.ShouldNotBeEmpty();
        user.EmailConfirmationCode.ShouldNotBeNull();

        await _userRepositoryMock.Received(1)
            .AddAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());

        await _unitOfWorkMock.Received(1)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
