using Application.Common.Constants;
using Application.Common.Results;
using Application.Common.Results.PredefinedErrors;
using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.DTOs.Response;
using Domain.Entities;
using Domain.StaticData;

namespace Application.Services.Authentication;

public sealed partial class AuthenticationService
{
    public async Task<Result<AuthResponse>> RefreshTokenAsync(
        RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        IDictionary<string, string>? claims = tokenReaderService.GetClaimsFromToken(request.AccessToken);

        if(claims is null)
            return Result<AuthResponse>.Failure(Errors.InvalidAccessToken);

        claims.TryGetValue(ClaimsConstants.NameIdentifier, out var nameIdentifier);
        claims.TryGetValue(ClaimsConstants.DeviceId, out var deviceId);

        if(string.IsNullOrEmpty(nameIdentifier) || string.IsNullOrEmpty(deviceId))
            return Result<AuthResponse>.Failure(Errors.InvalidAccessToken);

        if(!Guid.TryParse(claims[ClaimsConstants.NameIdentifier], out Guid userId))
            return Result<AuthResponse>.Failure(Errors.InvalidAccessToken);

        User? user = await userRepository.SingleOrDefaultAsync(
            x => x.Id == userId,
            cancellationToken: cancellationToken);

        if(user is null)
            return Result<AuthResponse>.Failure(Errors.InvalidAccessToken);

        var isRefreshTokenValid = await refreshTokenService.VerifyRefreshTokenAsync(
            userId,
            deviceId,
            request.RefreshToken,
            cancellationToken);

        if(!isRefreshTokenValid)
            return Result<AuthResponse>.Failure(Errors.InvalidRefreshToken);

        IEnumerable<RolePermission> rolePermissions = await rolePermissionsRepository.WhereAsync(
            x => x.RoleId == user.RoleId,
            cancellationToken: cancellationToken);

        IEnumerable<string> permissions = rolePermissions.Select(x => ((PermissionType)x.PermissionId).ToString());

        var tokenClaims = new TokenClaims
        {
            Email = user.Email,
            RoleId = user.RoleId,
            DeviceId = deviceId,
            UserId = user.Id.ToString(),
            Permissions = permissions,
        };

        var accessToken = tokenProvider.GenerateToken(tokenClaims);

        (var refreshToken, DateTime expiredAt) = await refreshTokenService.UpdateRefreshTokenAsync(
            userId,
            deviceId,
            cancellationToken);

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
