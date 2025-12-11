using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Product.Commands;
using Overclocked.Application.Product.Commands.CreateProduct;
using Overclocked.Application.Product.Commands.DeleteProduct;
using Overclocked.Application.Product.Commands.UpdateProduct;
using Overclocked.Application.Product.Queries;
using Overclocked.Application.Product.Queries.GetProduct;
using Overclocked.Contracts.Product;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;

namespace Overclocked.Api.Controllers;

[ApiController]
public class ProductController(IProductQueries productQueries, IProductCommands productCommands) : ControllerBase
{
    [HttpGet]
    [Route(ProductRoutes.GetById)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductQuery
        {
            Id = id
        };

        Result<ProductResponse> response = await productQueries.GetProductQueryHandler(query, cancellationToken);

        return response.ToActionResult(this);
    }

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
