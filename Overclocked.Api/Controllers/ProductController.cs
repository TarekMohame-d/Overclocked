using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Product.Commands;
using Overclocked.Application.Product.Commands.CreateProduct;
using Overclocked.Application.Product.Commands.DeleteProduct;
using Overclocked.Application.Product.Commands.UpdateProduct;
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

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPut]
    [Route(ProductRoutes.Update)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = UpdateProductCommand.Create(request, id);

        Result response = await productCommands.UpdateProductCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpDelete]
    [Route(ProductRoutes.Delete)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand
        {
            Id = id
        };

        Result response = await productCommands.DeleteProductCommandHandler(command, cancellationToken);

        return response.ToActionResult(this);
    }
}
