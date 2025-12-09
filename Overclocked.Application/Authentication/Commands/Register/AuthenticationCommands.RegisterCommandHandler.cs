using System.Net;
using Overclocked.Application.Authentication.Commands.Register;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;
using Overclocked.Domain.RoleAggregate.ValueObjects;
using Overclocked.Domain.UserAggregate;
using Overclocked.Domain.UserAggregate.ValueObjects;

namespace Overclocked.Application.Authentication.Commands;

public sealed partial class AuthenticationCommands
{
    public async Task<Result> RegisterCommandHandler(RegisterCommand command, CancellationToken cancellationToken)
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
