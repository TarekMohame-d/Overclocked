using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Authentication.Commands;
using Overclocked.Application.Authentication.Commands.ConfirmEmail;
using Overclocked.Application.Authentication.Commands.ForgetPassword;
using Overclocked.Application.Authentication.Commands.Login;
using Overclocked.Application.Authentication.Commands.RefreshToken;
using Overclocked.Application.Authentication.Commands.Register;
using Overclocked.Application.Authentication.Commands.ResendEmailConfirmationCode;
using Overclocked.Application.Authentication.Commands.ResetPassword;
using Overclocked.Contracts.Authentication;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Api.Controllers;

[ApiController]
public class AuthController(IAuthenticationCommands authenticationCommands) : ControllerBase
{
    [HttpPost]
    [Route(AuthRoutes.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand
        {
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber
        };

        Result response = await authenticationCommands.RegisterCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }

    [HttpPost]
    [Route(AuthRoutes.ConfirmEmail)]
    public async Task<IActionResult> ConfirmEmail(
        [FromBody] ConfirmEmailRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ConfirmEmailCommand
        {
            Email = request.Email,
            Code = request.Code
        };

        Result response = await authenticationCommands.ConfirmEmailCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }

    [HttpPost]
    [Route(AuthRoutes.Login)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password,
            DeviceId = request.DeviceId
        };

        Result<AuthResponse> response = await authenticationCommands.LoginCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }

    [HttpPost]
    [Route(AuthRoutes.ResendConfirmationCode)]
    public async Task<IActionResult> ResendConfirmationCode(
        [FromBody] ResendEmailConfirmationCodeRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ResendEmailConfirmationCodeCommand
        {
            Email = request.Email
        };

        Result response = await authenticationCommands.ResendConfirmationCodeCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }

    [HttpPost]
    [Route(AuthRoutes.ForgotPassword)]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ForgetPasswordCommand
        {
            Email = request.Email
        };

        Result response = await authenticationCommands.ForgetPasswordCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }

    [HttpPost]
    [Route(AuthRoutes.ResetPassword)]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ResetPasswordCommand
        {
            Email = request.Email,
            Password = request.Password,
            Code = request.Code
        };

        Result response = await authenticationCommands.ResetPasswordCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }

    [HttpPost]
    [Route(AuthRoutes.RefreshToken)]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand
        {
            AccessToken = request.AccessToken,
            RefreshToken = request.RefreshToken
        };

        Result<AuthResponse> response = await authenticationCommands
            .RefreshTokenCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }
}
