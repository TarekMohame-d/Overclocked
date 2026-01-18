using Overclocked.Application.Abstractions;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Persistence;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Features.AuthenticationUseCases.Common;
using Overclocked.Application.Features.AuthenticationUseCases.DTOs.Responses;
using Overclocked.Domain.UserAggregate;
using Overclocked.SharedKernel;

namespace Overclocked.Application.Features.AuthenticationUseCases.Login;

public class LoginRequestHandler(
    IAuthenticationRepository authenticationRepository,
    IUnitOfWork unitOfWork,
    IEmailConfirmationCodeService emailConfirmationCodeService,
    IPasswordHasher passwordHasher,
    IRefreshTokenHasher refreshTokenHasher,
    ITokenProvider tokenProvider
) : IRequestHandler<LoginRequest, AuthResponse>
{
    public async Task<Result<AuthResponse>> Handle(LoginRequest request, CancellationToken ct)
    {
        User? user = await authenticationRepository.GetByEmailAsync(request.Email, ct);

        var passwordHash =
            user?.PasswordHash
            ?? "1616DCA463F65B974204B57CBF28BD29F1FD75F8ECAB30836B003DD2D88820E4-AB3D8D1DAE09FF974C46FDF6BBD31A36";

        var isValid = passwordHasher.Verify(request.Password, passwordHash);

        if (user is null || !isValid)
            return Result.Failure<AuthResponse>(AuthenticationErrors.InvalidCredentials);

        if (!user.IsActive)
            return Result.Failure<AuthResponse>(AuthenticationErrors.UserIsInactive);

        if (!user.EmailConfirmed)
        {
            var code = emailConfirmationCodeService.GenerateVerificationCode();
            var codeHash = emailConfirmationCodeService.Hash(code);
            user.ResendEmailConfirmationCode(code, codeHash);

            await unitOfWork.SaveChangesAsync(ct);

            return Result.Failure<AuthResponse>(AuthenticationErrors.EmailNotConfirmed);
        }

        List<string> permissions = await authenticationRepository.GetPermissionsAsync(user.Role, ct);

        var tokenClaims = new TokenClaims(
            user.Id.Value.ToString(),
            user.Email,
            request.DeviceId.ToString(),
            user.Role.ToString(),
            permissions
        );

        var accessToken = tokenProvider.GenerateAccessToken(tokenClaims);
        var refreshToken = tokenProvider.GenerateRefreshToken();
        var refreshTokenHash = refreshTokenHasher.Hash(refreshToken);

        Result<DateTimeOffset> result = user.CreateRefreshToken(request.DeviceId, refreshTokenHash);

        if (result.IsFailure)
            return Result.Failure<AuthResponse>(result.Error);

        var authResponse = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiredAt = result.Value,
        };

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(authResponse);
    }
}
