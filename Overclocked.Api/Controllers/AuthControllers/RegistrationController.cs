using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Authentication.Commands.ConfirmEmail;
using Overclocked.Application.Authentication.Commands.Register;
using Overclocked.Application.Authentication.Commands.ResendEmailConfirmationCode;
using Overclocked.Contracts.Authentication;
using Overclocked.Domain.Common.Results;

namespace Overclocked.Api.Controllers.AuthControllers;

[ApiController]
public class RegistrationController(
    ICommandHandler<RegisterCommand> registerHandler,
    ICommandHandler<ConfirmEmailCommand> confirmHandler,
    ICommandHandler<ResendEmailConfirmationCodeCommand> resendHandler) : ControllerBase
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

        Result result = await registerHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Created,
            onFailure: error => error.ToProblemDetails(this));
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

        Result result = await confirmHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
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

        Result result = await resendHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }
}
