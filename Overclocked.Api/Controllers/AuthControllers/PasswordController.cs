using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Authentication.Commands.ForgetPassword;
using Overclocked.Application.Authentication.Commands.ResetPassword;
using Overclocked.Contracts.Authentication;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Api.Controllers.AuthControllers;

[ApiController]
public class PasswordController(
    ICommandHandler<ForgetPasswordCommand> forgetHandler,
    ICommandHandler<ResetPasswordCommand> resetHandler) : ControllerBase
{
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

        Result result = await forgetHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
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

        Result result = await resetHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }
}
