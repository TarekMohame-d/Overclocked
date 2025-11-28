using Api.ActionFilters;
using Api.Extensions;
using Api.Routing;
using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Cart.DTOs.Request;
using Application.Services.Cart.DTOs.Response;
using Domain.StaticData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class CartController(ICartService cartService) : ControllerBase
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
        Result<CartItemResponse> response = await cartService
            .GetCartItemsAsync((Guid)userId, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<AddCartItemRequestBody>))]
    [Route(CartRoutes.AddCartItem)]
    public async Task<IActionResult> AddCartItem(
        [FromBody] AddCartItemRequestBody requestBody,
        CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var request = new AddCartItemRequest
        {
            ProductId = requestBody.ProductId,
            Quantity = requestBody.Quantity,
            UserId = (Guid)userId
        };

        Result response = await cartService.AddCartItemAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpPut]
    [ServiceFilter(typeof(ValidationActionAttribute<UpdateCartItemRequestBody>))]
    [Route(CartRoutes.UpdateCartItem)]
    public async Task<IActionResult> UpdateCartItem(
        [FromRoute] Guid id,
        [FromBody] UpdateCartItemRequestBody requestBody,
        CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }

        var request = new UpdateCartItemRequest
        {
            Quantity = requestBody.Quantity,
            CartItemId = id,
            UserId = (Guid)userId
        };

        Result response = await cartService.UpdateCartItemAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Roles = nameof(RoleType.Customer))]
    [HttpDelete]
    [Route(CartRoutes.DeleteCartItem)]
    public async Task<IActionResult> DeleteCartItem([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        Guid? userId = HttpContext.GetUserId();
        if(userId == null)
        {
            return Unauthorized();
        }
        var request = new DeleteCartItemRequest
        {
            CartItemId = id,
            UserId = (Guid)userId
        };

        Result response = await cartService.DeleteCartItemAsync(request, cancellationToken);

        return response.ToActionResult();
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

        Result response = await cartService.ClearCartAsync((Guid)userId, cancellationToken);

        return response.ToActionResult();
    }
}
