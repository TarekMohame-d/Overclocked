using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.WishlistUseCases.AddWishlistItem;
using Overclocked.Application.Features.WishlistUseCases.ClearWishlist;
using Overclocked.Application.Features.WishlistUseCases.DeleteWishlistItem;
using Overclocked.Application.Features.WishlistUseCases.DTOs.Requests;
using Overclocked.Application.Features.WishlistUseCases.DTOs.Responses;
using Overclocked.Application.Features.WishlistUseCases.GetWishlistItems;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.SharedKernel;

namespace Overclocked.Api.Controllers;

public class WishlistController(IDispatcher dispatcher) : ControllerBase
{
    [Authorize(Roles = nameof(Role.Customer))]
    [HttpGet]
    [Route(WishlistRoutes.GetWishlistItems)]
    public async Task<IActionResult> GetWishlistItems(CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new GetWishlistItemsRequest { UserId = (Guid)userId };

        Result<IEnumerable<WishlistItemResponse>> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpPost]
    [Route(WishlistRoutes.AddWishlistItem)]
    public async Task<IActionResult> AddWishlistItems([FromBody] AddWishlistItemRequestDto dto, CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new AddWishlistItemRequest { UserId = (Guid)userId, ProductId = dto.ProductId };

        Result<IEnumerable<WishlistItemResponse>> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpDelete]
    [Route(WishlistRoutes.DeleteWishlistItem)]
    public async Task<IActionResult> DeleteWishlistItem([FromRoute] Guid id, CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new DeleteWishlistItemRequest { UserId = (Guid)userId, ProductId = id };

        Result<IEnumerable<WishlistItemResponse>> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Roles = nameof(Role.Customer))]
    [HttpDelete]
    [Route(WishlistRoutes.ClearWishlist)]
    public async Task<IActionResult> ClearWishlist(CancellationToken ct)
    {
        Guid? userId = HttpContext.GetUserId();
        if (userId is null)
            return Unauthorized();

        var request = new ClearWishlistRequest { UserId = (Guid)userId };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }
}
