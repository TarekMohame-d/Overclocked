using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Cart.Commands.AddCartItem;
using Overclocked.Application.Cart.Commands.ClearCart;
using Overclocked.Application.Cart.Commands.DeleteCartItem;
using Overclocked.Application.Cart.Commands.UpdateCartItem;
using Overclocked.Application.Cart.Queries.GetCartItems;
using Overclocked.Contracts.Cart;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;

namespace Overclocked.Api.Controllers;

[ApiController]
public class CartController(
    ICommandHandler<AddCartItemCommand, CartResponse> addHandler,
    ICommandHandler<UpdateCartItemCommand, CartResponse> updateHandler,
    ICommandHandler<DeleteCartItemCommand, CartResponse> deleteHandler,
    ICommandHandler<ClearCartCommand> clearHandler,
    IQueryHandler<GetCartItemsQuery, CartResponse> queryHandler) : ControllerBase
{
    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpGet]
    [Route(CartRoutes.GetCartItems)]
    public async Task<IActionResult> GetCartItems(CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var query = new GetCartItemsQuery
        {
            UserId = (Guid)userId
        };

        Result<CartResponse> result = await queryHandler.Handle(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpPost]
    [Route(CartRoutes.AddCartItem)]
    public async Task<IActionResult> AddCartItem(
        [FromBody] AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var command = new AddCartItemCommand
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            UserId = (Guid)userId
        };

        Result<CartResponse> result = await addHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpPut]
    [Route(CartRoutes.UpdateCartItem)]
    public async Task<IActionResult> UpdateCartItem(
        [FromRoute] Guid id,
        [FromBody] UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var command = new UpdateCartItemCommand
        {
            CartItemId = id,
            Quantity = request.Quantity,
            UserId = (Guid)userId
        };

        Result<CartResponse> result = await updateHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpDelete]
    [Route(CartRoutes.DeleteCartItem)]
    public async Task<IActionResult> DeleteCartItem(
    [FromRoute] Guid id,
    CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var command = new DeleteCartItemCommand
        {
            CartItemId = id,
            UserId = (Guid)userId
        };

        Result<CartResponse> result = await deleteHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpDelete]
    [Route(CartRoutes.ClearCart)]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var command = new ClearCartCommand
        {
            UserId = (Guid)userId
        };

        Result result = await clearHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }
}
