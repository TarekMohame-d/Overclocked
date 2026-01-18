using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.CartUseCases.AddCartItem;
using Overclocked.Application.Features.CartUseCases.ClearCart;
using Overclocked.Application.Features.CartUseCases.DeleteCartItem;
using Overclocked.Application.Features.CartUseCases.DTOs.Requests;
using Overclocked.Application.Features.CartUseCases.DTOs.Responses;
using Overclocked.Application.Features.CartUseCases.GetCartItems;
using Overclocked.Application.Features.CartUseCases.UpdateCartItem;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.SharedKernel;

namespace Overclocked.Api.Controllers;

[ApiController]
public class CartController(IDispatcher dispatcher) : ControllerBase
{
    [Authorize(Roles = nameof(Role.Customer))]
    [HttpGet]
    [Route(CartRoutes.GetCartItems)]
    public async Task<IActionResult> GetCartItems(CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new GetCartItemsRequest { UserId = (Guid)userId };

        Result<CartResponse> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpPost]
    [Route(CartRoutes.AddCartItem)]
    public async Task<IActionResult> AddCartItem([FromBody] AddCartItemRequestDto dto, CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new AddCartItemRequest
        {
            ProductId = dto.ProductId,
            Quantity = dto.Quantity,
            UserId = userId.Value,
        };

        Result<CartResponse> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpPut]
    [Route(CartRoutes.UpdateCartItem)]
    public async Task<IActionResult> UpdateCartItem(
        [FromRoute] Guid id,
        [FromBody] UpdateCartItemRequestDto dto,
        CancellationToken ct
    )
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new UpdateCartItemRequest
        {
            CartItemId = id,
            Quantity = dto.Quantity,
            UserId = userId.Value,
        };

        Result<CartResponse> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpDelete]
    [Route(CartRoutes.DeleteCartItem)]
    public async Task<IActionResult> DeleteCartItem([FromRoute] Guid id, CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new DeleteCartItemRequest { CartItemId = id, UserId = (Guid)userId };

        Result<CartResponse> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpDelete]
    [Route(CartRoutes.ClearCart)]
    public async Task<IActionResult> ClearCart(CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new ClearCartRequest { UserId = userId.Value };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }
}
