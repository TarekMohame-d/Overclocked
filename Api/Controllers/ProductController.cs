using Api.ActionFilters;
using Api.Extensions;
using Api.Routing;
using Application.Abstraction.Services;
using Application.Services.Product.DTOs.Request;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [Route(ProductRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetProductByIdRequest { Id = id };
        var response = await _productService.GetProductByIdAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [HttpGet]
    [ServiceFilter(typeof(ValidationActionAttribute<GetPagedProductsRequest>))]
    [Route(ProductRoutes.GetAll)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPagedProductsRequest request,
        CancellationToken cancellationToken)
    {
        var query = GetPagedProductsQuery.FromRequest(request);
        var response = await _productService.GetPagedProductsAsync(query, cancellationToken);

        return response.ToActionResult();
    }

    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<CreateProductRequest>))]
    [Route(ProductRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var response = await _productService.CreateProductAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpPut]
    [ServiceFilter(typeof(ValidationActionAttribute<UpdateProductRequest>))]
    [Route(ProductRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        request = request with { Id = id };

        var response = await _productService.UpdateProductAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpDelete]
    [Route(ProductRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteProductRequest { Id = id };
        var response = await _productService.DeleteProductAsync(request, cancellationToken);

        return response.ToActionResult();
    }
}
