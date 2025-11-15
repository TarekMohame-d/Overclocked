using Api.ActionFilters;
using Api.Extensions;
using Api.Routing;
using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Authentication.DTOs.Request;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class AuthController(IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<RegisterRequest>))]
    [Route(AuthRoutes.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        Result response = await authenticationService.RegisterAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<ConfirmEmailRequest>))]
    [Route(AuthRoutes.ConfirmEmail)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request,
        CancellationToken cancellationToken)
    {
        Result response = await authenticationService.ConfirmEmailAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<LoginRequest>))]
    [Route(AuthRoutes.Login)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        Result response = await authenticationService.LoginAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<ResendEmailConfirmationCodeRequest>))]
    [Route(AuthRoutes.ResendConfirmationCode)]
    public async Task<IActionResult> ResendConfirmationCode([FromBody] ResendEmailConfirmationCodeRequest request,
        CancellationToken cancellationToken)
    {
        Result response = await authenticationService.ResendEmailConfirmationCodeAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<ForgetPasswordRequest>))]
    [Route(AuthRoutes.ForgotPassword)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        Result response = await authenticationService.ForgetPasswordAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<ResetPasswordRequest>))]
    [Route(AuthRoutes.ResetPassword)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        Result response = await authenticationService.ResetPasswordAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<RefreshTokenRequest>))]
    [Route(AuthRoutes.RefreshToken)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        Result response = await authenticationService.RefreshTokenAsync(request, cancellationToken);

        return response.ToActionResult();
    }
}
