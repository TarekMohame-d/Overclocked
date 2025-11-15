using Application.Common.Results;
using Application.Services.Authentication.DTOs.Request;
using Application.Services.Authentication.DTOs.Response;

namespace Application.Abstraction.DomainServices;

public interface IAuthenticationService
{
    Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
    Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request, CancellationToken cancellationToken);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

    Task<Result> ResendEmailConfirmationCodeAsync(ResendEmailConfirmationCodeRequest request,
        CancellationToken cancellationToken);

    Task<Result> ForgetPasswordAsync(ForgetPasswordRequest request,
        CancellationToken cancellationToken);

    Task<Result> ResetPasswordAsync(ResetPasswordRequest request,
        CancellationToken cancellationToken);

    Task<Result<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request,
        CancellationToken cancellationToken);
}
