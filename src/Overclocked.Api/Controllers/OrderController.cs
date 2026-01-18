using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.OrderUseCases.CancelOrder;
using Overclocked.Application.Features.OrderUseCases.CreateOrder;
using Overclocked.Application.Features.OrderUseCases.DTOs.Requests;
using Overclocked.Application.Features.OrderUseCases.DTOs.Responses;
using Overclocked.Application.Features.OrderUseCases.GetPagedOrders;
using Overclocked.Application.Features.OrderUseCases.RetryOrderPayment;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.SharedKernel;

namespace Overclocked.Api.Controllers;

[ApiController]
public class OrderController(IDispatcher dispatcher) : ControllerBase
{
    [Authorize(Roles = nameof(Role.Customer))]
    [HttpGet]
    [Route(OrderRoutes.GetPagedOrders)]
    public async Task<IActionResult> GetPaged(
        [FromRoute] int year,
        [FromQuery] GetPagedOrdersRequestDto dto,
        CancellationToken ct
    )
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        Result<GetPagedOrdersRequest> validationResult = GetPagedOrdersRequest.FromDto(dto, year, userId.Value);

        if (validationResult.IsFailure)
        {
            return validationResult.Match(onSuccess: _ => null!, onFailure: error => error.ToProblemDetails(this));
        }

        Result<PagedResult<OrderPagedResponse>> result = await dispatcher.Send(validationResult.Value, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpPost]
    [Route(OrderRoutes.CreateOrder)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequestDto dto, CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        Result<CreateOrderRequest> validationResult = CreateOrderRequest.FromDto(dto, userId.Value);

        if (validationResult.IsFailure)
        {
            return validationResult.Match(onSuccess: _ => null!, onFailure: error => error.ToProblemDetails(this));
        }

        Result<CreateOrderResponse> result = await dispatcher.Send(validationResult.Value, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpPost]
    [Route(OrderRoutes.RetryOrder)]
    public async Task<IActionResult> Retry([FromRoute] Guid id, [FromBody] RetryOrderPaymentRequestDto dto, CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        Result<RetryOrderPaymentRequest> validationResult = RetryOrderPaymentRequest.FromDto(dto, userId.Value, id);

        if (validationResult.IsFailure)
        {
            return validationResult.Match(onSuccess: _ => null!, onFailure: error => error.ToProblemDetails(this));
        }

        Result<CreateOrderResponse> result = await dispatcher.Send(validationResult.Value, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpPost]
    [Route(OrderRoutes.CancelOrder)]
    public async Task<IActionResult> Cancel([FromRoute] Guid id, [FromBody] CancelOrderRequestDto dto, CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new CancelOrderRequest
        {
            UserId = userId.Value,
            OrderId = id,
            RefundToWallet = dto.RefundToWallet,
        };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }
}
