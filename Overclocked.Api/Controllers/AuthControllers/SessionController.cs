using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Authentication.Commands.Login;
using Overclocked.Application.Authentication.Commands.RefreshToken;
using Overclocked.Contracts.Authentication;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Api.Controllers.AuthControllers;

[ApiController]
public class SessionController(
    ICommandHandler<LoginCommand, AuthResponse> loginHandler,
    ICommandHandler<RefreshTokenCommand, AuthResponse> refreshHandler) : ControllerBase
{
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

        Result<AuthResponse> result = await loginHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
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

        Result<AuthResponse> result = await refreshHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }
}
