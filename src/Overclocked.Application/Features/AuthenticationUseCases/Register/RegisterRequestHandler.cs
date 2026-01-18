using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Domain.UserAggregate;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.AuthenticationUseCases.Register;

public class RegisterRequestHandler(
    IAuthenticationRepository authenticationRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IEmailConfirmationCodeService emailConfirmationCodeService
) : IRequestHandler<RegisterRequest>
{
    public async Task<Result> Handle(RegisterRequest request, CancellationToken ct)
    {
        if (await authenticationRepository.PhoneExistsAsync(request.PhoneNumber, ct))
            return Result.Failure(AuthenticationErrors.PhoneAlreadyExists);

        if (await authenticationRepository.EmailExistsAsync(request.Email, ct))
            return Result.Failure(AuthenticationErrors.EmailAlreadyExists);

        var passwordHash = passwordHasher.Hash(request.Password);
        var code = emailConfirmationCodeService.GenerateVerificationCode();
        var codeHash = emailConfirmationCodeService.Hash(code);

        Result<User> result = User.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            passwordHash,
            request.PhoneNumber,
            code,
            codeHash
        );

        if (result.IsFailure)
            return Result.Failure(result.Error);

        User user = result.Value;

        authenticationRepository.Add(user);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
