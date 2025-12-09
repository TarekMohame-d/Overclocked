using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Product.Commands;
using Overclocked.Application.Product.Commands.CreateProduct;
using Overclocked.Contracts.Product;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;

namespace Overclocked.Api.Controllers;

[ApiController]
public class ProductController(IProductCommands productCommands) : ControllerBase
{
    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPost]
    [Route(ProductRoutes.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = CreateProductCommand.Create(request);

        Result response = await productCommands.CreateProductCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }
}
