using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.AuthenticationUseCases.ForgetPassword;
using Overclocked.Application.Features.AuthenticationUseCases.ResetPassword;
using Overclocked.SharedKernel;

namespace Overclocked.Api.Controllers.AuthControllers;

[ApiController]
public class PasswordController(IDispatcher dispatcher) : ControllerBase
{
    [HttpPost]
    [Route(AuthRoutes.ForgotPassword)]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgetPasswordRequestBody body, CancellationToken ct)
    {
        var request = new ForgetPasswordRequest { Email = body.Email };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [HttpPost]
    [Route(AuthRoutes.ResetPassword)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestBody body, CancellationToken ct)
    {
        var request = new ResetPasswordRequest
        {
            Email = body.Email,
            Password = body.Password,
            Code = body.Code,
        };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    public record ForgetPasswordRequestBody(string Email);

    public record ResetPasswordRequestBody(string Email, string Password, string Code);
}
