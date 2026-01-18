using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Factories;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Abstractions.Services;
using Overclocked.Application.Common.Constants.Payment;
using Overclocked.Application.Features.PaymentUseCases.DTOs.Responses;
using Overclocked.Application.Features.PaymentUseCases.GetPayments;
using Overclocked.Infrastructure.BackgroundJobs;
using Overclocked.SharedKernel;

namespace Overclocked.Api.Controllers;

[ApiController]
public class PaymentController(IDispatcher dispatcher, PaymentFactory paymentFactory) : ControllerBase
{
    [Authorize]
    [HttpGet]
    [Route(PaymentRoutes.GetPayments)]
    public async Task<IActionResult> GetPayments(CancellationToken ct)
    {
        var request = new GetPaymentsRequest();

        Result<PaymentResponse> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [HttpPost]
    [Route(PaymentRoutes.PaymobCallback)]
    public async Task<IActionResult> PaymobCallback()
    {
        using var reader = new StreamReader(Request.Body);
        var rawBody = await reader.ReadToEndAsync();

        IPaymentProviderService provider = paymentFactory.GetProvider(PaymentProvider.Paymob);

        Result result = await provider.ProcessCallback(rawBody, Request.Headers, Request.Query);

        if (result.IsFailure)
        {
            if (result.Error.Code == "Paymob.Security" || result.Error.Code == "Paymob.Callback")
            {
                return BadRequest(result.Error);
            }

            return StatusCode(500);
        }

        BackgroundJob.Enqueue<ProcessPendingWebhooksJob>(job => job.ProcessPendingWebhooksAsync());

        return Ok();
    }
}
