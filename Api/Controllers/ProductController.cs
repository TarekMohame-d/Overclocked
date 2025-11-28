using Api.ActionFilters;
using Api.Extensions;
using Api.Routing;
using Application.Abstraction.DomainServices;
using Application.Common.Results;
using Application.Services.Product.DTOs.Request;
using Application.Services.Product.DTOs.Response;
using Domain.StaticData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet]
    [Route(ProductRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetProductByIdRequest { Id = id };
        Result<ProductResponse> response = await productService.GetProductByIdAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [HttpGet]
    [ServiceFilter(typeof(ValidationActionAttribute<GetPagedProductsQuery>))]
    [Route(ProductRoutes.GetAll)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPagedProductsQuery query,
        CancellationToken cancellationToken)
    {
        var request = GetPagedProductsRequest.FromQuery(query);
        Result<PagedResult<ProductListResponse>> response = await productService.GetPagedProductsAsync(
            request,
            cancellationToken
        );

        return response.ToActionResult();
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<CreateProductRequest>))]
    [Route(ProductRoutes.Create)]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        Result response = await productService.CreateProductAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpPut]
    [ServiceFilter(typeof(ValidationActionAttribute<UpdateProductRequestBody>))]
    [Route(ProductRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateProductRequestBody request,
        CancellationToken cancellationToken)
    {
        var updateProductRequest = UpdateProductRequest.FromBody(request, id);

        Result response = await productService.UpdateProductAsync(updateProductRequest, cancellationToken);

        return response.ToActionResult();
    }

    [Authorize(Policy = nameof(PermissionType.AddEditDelete))]
    [HttpDelete]
    [Route(ProductRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        Result response = await productService.DeleteProductAsync(id, cancellationToken);

        return response.ToActionResult();
    }
}
