using Api.ActionFilters;
using Api.Extensions;
using Api.Routing;
using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Wishlist.DTOs.Request;
using Application.Services.Wishlist.DTOs.Response;
using Domain.StaticData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class WishlistController(IWishlistService wishlistService) : ControllerBase
{
    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpGet]
    [Route(WishlistRoutes.GetWishlistItems)]
    public async Task<IActionResult> GetWishlistItems(CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }
        Result<IEnumerable<WishlistItemResponse>> response = await wishlistService
            .GetWishlistItemsAsync((Guid)userId, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<AddWishlistItemRequest>))]
    [Route(WishlistRoutes.AddWishlistItem)]
    public async Task<IActionResult> AddWishlistItem(
        [FromBody] AddWishlistItemRequest request,
        CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }
        Result response = await wishlistService.AddWishlistItemAsync((Guid)userId, request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpDelete]
    [Route(WishlistRoutes.DeleteWishlistItem)]
    public async Task<IActionResult> DeleteWishlistItem([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }
        Result response = await wishlistService.DeleteWishlistItemAsync((Guid)userId, id, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpDelete]
    [Route(WishlistRoutes.ClearWishlist)]
    public async Task<IActionResult> ClearWishlist(CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        Result response = await wishlistService.ClearWishlistAsync((Guid)userId, cancellationToken);

        return response.ToActionResult();
    }
}
