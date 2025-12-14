using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Product.Commands.CreateProduct;
using Overclocked.Application.Product.Commands.DeleteProduct;
using Overclocked.Application.Product.Commands.UpdateProduct;
using Overclocked.Application.Product.Queries.GetPagedProducts;
using Overclocked.Application.Product.Queries.GetProductById;
using Overclocked.Contracts.Product;
using Overclocked.Domain.Common.Results;
using Overclocked.Domain.Common.StaticData;

namespace Overclocked.Api.Controllers;

[ApiController]
public class ProductController(ICommandHandler<CreateProductCommand> createHandler,
    ICommandHandler<UpdateProductCommand> updateHandler,
    ICommandHandler<DeleteProductCommand> deleteHandler,
    IQueryHandler<GetProductByIdQuery, ProductResponse> getByIdHandler,
    IQueryHandler<GetPagedProductsQuery, PagedResult<ProductPagedResponse>> getPagedHandler) : ControllerBase
{
    [HttpGet]
    [Route(ProductRoutes.GetById)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery
        {
            Id = id
        };

        Result<ProductResponse> result = await getByIdHandler.Handle(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [HttpGet]
    [Route(ProductRoutes.GetPaged)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] GetPagedProductsRequest request,
        CancellationToken cancellationToken)
    {
        var query = GetPagedProductsQuery.ToQuery(request);

        Result<PagedResult<ProductPagedResponse>> result = await getPagedHandler
            .Handle(query, cancellationToken);

        return result.Match(
            onSuccess: Ok,
            onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPost]
    [Route(ProductRoutes.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = CreateProductCommand.Create(request);

        Result result = await createHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: Created,
            onFailure: error => error.ToProblemDetails(this));
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

        Result result = await updateHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: error => error.ToProblemDetails(this));
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

        Result result = await deleteHandler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: NoContent,
            onFailure: error => error.ToProblemDetails(this));
    }
}
