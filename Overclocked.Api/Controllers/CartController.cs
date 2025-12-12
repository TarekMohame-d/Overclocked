using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Cart.Commands;
using Overclocked.Application.Cart.Commands.AddCartItem;
using Overclocked.Contracts.Cart;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;

namespace Overclocked.Api.Controllers;

[ApiController]
public class CartController(ICartCommands cartCommands) : ControllerBase
{
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

        Result<CartItemResponse> response = await cartCommands.AddCartItemCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }
}
