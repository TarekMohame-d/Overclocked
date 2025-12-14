using System.Net;
using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;
using Overclocked.Domain.RoleAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Application.Authentication.Commands.Register;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IEmailConfirmationCodeService emailConfirmationCodeService) : ICommandHandler<RegisterCommand>
{
    public async Task<Result> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        var passwordHash = passwordHasher.Hash(command.Password);
        var code = emailConfirmationCodeService.GenerateVerificationCode();
        var codeHash = emailConfirmationCodeService.Hash(code);

        var roleId = RoleId.Create((int)RoleType.Customer);

        var user = User.Create(
            UserId.Create(),
            roleId,
            command.FirstName,
            command.LastName,
            command.Email,
            passwordHash,
            command.PhoneNumber,
            code,
            codeHash);

        await userRepository.AddAsync(user, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
