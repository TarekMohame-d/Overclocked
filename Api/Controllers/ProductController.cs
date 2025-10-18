using Api.Extensions;
using Api.Routing;
using Application.Abstraction.Messaging;
using Application.Common.Results;
using Application.Features.Brand.Commands.CreateBrand;
using Application.Features.Product.Commands.CreateProduct;
using Application.Features.Product.Commands.DeleteProduct;
using Application.Features.Product.Commands.UpdateProduct;
using Application.Features.Product.Queries.GetPagedProducts;
using Application.Features.Product.Queries.GetProductById;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route(ProductRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery { Id = id };
        Result<ProductDto> response = await _mediator.Send(query, cancellationToken);

        return response.ToActionResult();
    }

    [HttpGet]
    [Route(ProductRoutes.GetAll)]
    public async Task<IActionResult> GetAll(
        [FromQuery] GetPagedProductsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetPagedProductsQuery
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy
        };

        var response = await _mediator.Send(query, cancellationToken);

        return response.ToActionResult();
    }

    [HttpPost]
    [Route(ProductRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpPut]
    [Route(ProductRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductWithIdCommand
        {
            Id = id,
            BrandId = request.BrandId,
            CategoryId = request.CategoryId,
            Name = request.Name,
            Thumbnail = request.Thumbnail,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            Discount = request.Discount,
            Tags = request.Tags,
            Images = request.Images,
            Specification = request.Specification
        };

        var response = await _mediator.Send(command, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpDelete]
    [Route(ProductRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand { Id = id };
        var response = await _mediator.Send(command, cancellationToken);

        return response.ToActionResult();
    }
}
