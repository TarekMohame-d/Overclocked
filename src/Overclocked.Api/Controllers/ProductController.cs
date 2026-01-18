using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Overclocked.Api.Extensions;
using Overclocked.Api.Routing;
using Overclocked.Application.Abstractions.Messaging;
using Overclocked.Application.Features.ProductUseCases.CreateProduct;
using Overclocked.Application.Features.ProductUseCases.DeleteProduct;
using Overclocked.Application.Features.ProductUseCases.DTOs.Requests;
using Overclocked.Application.Features.ProductUseCases.DTOs.Responses;
using Overclocked.Application.Features.ProductUseCases.GetPagedProducts;
using Overclocked.Application.Features.ProductUseCases.GetProductById;
using Overclocked.Application.Features.ProductUseCases.UpdateProduct;
using Overclocked.Domain.UserAggregate.Enums;
using Overclocked.SharedKernel;

namespace Overclocked.Api.Controllers;

[ApiController]
public class ProductController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    [Route(ProductRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new GetProductByIdRequest { Id = id };

        Result<ProductResponse> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [HttpGet]
    [Route(ProductRoutes.GetPaged)]
    public async Task<IActionResult> GetPaged([FromQuery] GetPagedProductsQuery query, CancellationToken ct)
    {
        var request = GetPagedProductsRequest.FromRequest(query);

        Result<PagedResult<ProductPagedResponse>> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: Ok, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpPost]
    [Route(ProductRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequestDto dto, CancellationToken ct)
    {
        var request = CreateProductRequest.FromDto(dto);

        Result<Guid> result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: x => Created(string.Empty, x), onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpPut]
    [Route(ProductRoutes.Update)]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateProductRequestDto dto, CancellationToken ct)
    {
        var request = UpdateProductRequest.FromDto(dto, id);

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: NoContent, onFailure: error => error.ToProblemDetails(this));
    }

    [Authorize(Policy = nameof(Permission.AddEditDelete))]
    [HttpDelete]
    [Route(ProductRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        var request = new DeleteProductRequest { Id = id };

        Result result = await dispatcher.Send(request, ct);

        return result.Match(onSuccess: NoContent, onFailure: error => error.ToProblemDetails(this));
    }
}
