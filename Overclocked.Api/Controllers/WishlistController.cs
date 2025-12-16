using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Wishlist.Commands.AddWishlistItem;
using Overclocked.Application.Wishlist.Commands.ClearWishlist;
using Overclocked.Application.Wishlist.Commands.DeleteWishlistItem;
using Overclocked.Application.Wishlist.Queries.GetWishlistItems;
using Overclocked.Contracts.Wishlist;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.UserAggregate.Enums;

namespace Overclocked.Api.Controllers;

public class WishlistController(
    ICommandHandler<AddWishlistItemCommand, WishlistResponse> addHandler,
    ICommandHandler<DeleteWishlistItemCommand, WishlistResponse> deleteHandler,
    ICommandHandler<ClearWishlistCommand> clearHandler,
    IQueryHandler<GetWishlistItemsQuery, WishlistResponse> queryHandler) : ControllerBase
{
    [Authorize(Roles = nameof(Role.Customer))]
    [HttpGet]
    [Route(WishlistRoutes.GetWishlistItems)]
    public async Task<IActionResult> GetWishlistItems(CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var query = new GetWishlistItemsQuery
        {
            UserId = (Guid)userId
        };

        Result<WishlistResponse> result = await queryHandler.Handle(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpPost]
    [Route(WishlistRoutes.AddWishlistItem)]
    public async Task<IActionResult> AddWishlistItems(
        [FromBody] AddWishlistItemRequest request,
        CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var command = new AddWishlistItemCommand
        {
            UserId = (Guid)userId,
            ProductId = request.ProductId
        };

        Result<WishlistResponse> result = await addHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpDelete]
    [Route(WishlistRoutes.DeleteWishlistItem)]
    public async Task<IActionResult> DeleteWishlistItem(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var command = new DeleteWishlistItemCommand
        {
            UserId = (Guid)userId,
            ProductId = id
        };

        Result<WishlistResponse> result = await deleteHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpDelete]
    [Route(WishlistRoutes.ClearWishlist)]
    public async Task<IActionResult> ClearWishlist(CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var command = new ClearWishlistCommand
        {
            UserId = (Guid)userId
        };

        Result result = await clearHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }
}
