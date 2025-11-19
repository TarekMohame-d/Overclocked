using System.Net;
using Application.Common.Results;
using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.Events;
using Application.Services.Authentication.Mapping;
using Domain.Entities;

namespace Application.Services.Authentication;

public sealed partial class AuthenticationService
{
    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken)
    {
        var passwordHash = passwordHasher.Hash(request.Password);

        User user = request.ToEntity(passwordHash);

        await userRepository.AddAsync(user, cancellationToken);
        var code = await emailConfirmationCodeService.CreateEmailConfirmationCodeAsync(user.Id, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        var userRegisteredEvent = new UserRegisteredEvent(user.Email, code);
        await eventDispatcher.DispatchAsync(userRegisteredEvent, cancellationToken);

        return Result.Success(HttpStatusCode.Created);
    }
}
