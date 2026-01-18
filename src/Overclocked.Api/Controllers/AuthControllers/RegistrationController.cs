using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.AuthenticationUseCases.ConfirmEmail;
using Overclocked.Application.Features.AuthenticationUseCases.Register;
using Overclocked.Application.Features.AuthenticationUseCases.ResendEmailConfirmationCode;
using Overclocked.SharedKernel;

namespace Overclocked.Api.Controllers.AuthControllers;

[ApiController]
public class RegistrationController(IDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    [Route(AuthRoutes.Register)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestBody body, CancellationToken ct)
    {
        var request = new RegisterRequest
        {
            Email = body.Email,
            Password = body.Password,
            FirstName = body.FirstName,
            LastName = body.LastName,
            PhoneNumber = body.PhoneNumber,
        };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Created, onFailure: error => error.ToProblemDetails(this));
    }

    [HttpPost]
    [Route(AuthRoutes.ConfirmEmail)]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequestBody body, CancellationToken ct)
    {
        var request = new ConfirmEmailRequest { Email = body.Email, Code = body.Code };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [HttpPost]
    [Route(AuthRoutes.ResendConfirmationCode)]
    public async Task<IActionResult> ResendConfirmationCode(
        [FromBody] ResendEmailConfirmationCodeRequestBody body,
        CancellationToken ct
    )
    {
        var request = new ResendEmailConfirmationCodeRequest { Email = body.Email };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    public record RegisterRequestBody(string Email, string Password, string FirstName, string LastName, string PhoneNumber);

    public record ConfirmEmailRequestBody(string Email, string Code);

    public record ResendEmailConfirmationCodeRequestBody(string Email);
}
