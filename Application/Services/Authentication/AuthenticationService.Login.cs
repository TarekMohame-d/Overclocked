using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.DTOs.Response;
using Application.Services.Authentication.Events;
using Domain.Entities;
using Domain.StaticData;

namespace Application.Services.Authentication;

public sealed partial class AuthenticationService
{
    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        // TODO: refactor to use user service instead of directly using user repository
        User? user = await userRepository.SingleOrDefaultAsync(
            x => x.Email == request.Email,
            cancellationToken: cancellationToken);

        var passwordHash =
            user?.PasswordHash
            ?? "1616DCA463F65B974204B57CBF28BD29F1FD75F8ECAB30836B003DD2D88820E4-AB3D8D1DAE09FF974C46FDF6BBD31A36";

        var isValid = passwordHasher.Verify(request.Password, passwordHash);

        if(user is null || !isValid)
            return Result<AuthResponse>.Failure(Errors.InvalidCredentials);

        if(!user.EmailConfirmed)
        {
            var emailNotConfirmedEvent = new EmailNotConfirmedEvent(user.Email, user.Id);
            await eventDispatcher.DispatchAsync(emailNotConfirmedEvent, cancellationToken);
            return Result<AuthResponse>.Failure(Errors.EmailNotConfirmed);
        }

        IEnumerable<RolePermission> rolePermissions = await rolePermissionsRepository.WhereAsync(
            x => x.RoleId == user.RoleId,
            cancellationToken: cancellationToken);

        IEnumerable<string> permissions = rolePermissions.Select(x => ((PermissionType)x.PermissionId).ToString());

        var tokenClaims = new TokenClaims
        {
            Email = user.Email,
            RoleId = user.RoleId,
            DeviceId = request.DeviceId,
            UserId = user.Id.ToString(),
            Permissions = permissions,
        };

        var accessToken = tokenProvider.GenerateToken(tokenClaims);

        (var refreshToken, DateTime expiredAt) = await refreshTokenService
            .CreateRefreshTokenAsync(user.Id, request.DeviceId, cancellationToken);

        var authResponse = new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiration = expiredAt,
        };

        await unitOfWork.CompleteAsync(cancellationToken);

        return Result<AuthResponse>.Success(authResponse);
    }
}
