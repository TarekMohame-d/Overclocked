using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.AuthenticationUseCases.DTOs.Requests;
using Overclocked.Application.Features.AuthenticationUseCases.DTOs.Responses;
using Overclocked.Application.Features.AuthenticationUseCases.Login;
using Overclocked.Application.Features.AuthenticationUseCases.RefreshToken;
using Overclocked.SharedKernel;

namespace Overclocked.Api.Controllers.AuthControllers;

[ApiController]
public class SessionController(IDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    [Route(AuthRoutes.Login)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto, CancellationToken ct)
    {
        var request = new LoginRequest
        {
            Email = dto.Email,
            Password = dto.Password,
            DeviceId = dto.DeviceId,
        };

        Result<AuthResponse> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [HttpPost]
    [Route(AuthRoutes.RefreshToken)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto, CancellationToken ct)
    {
        var request = new RefreshTokenRequest { AccessToken = dto.AccessToken, RefreshToken = dto.RefreshToken };

        Result<AuthResponse> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }
}
