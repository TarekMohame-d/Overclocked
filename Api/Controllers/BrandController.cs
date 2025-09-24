using Api.Common.Routing;
using Api.Extensions;
using Application.Abstraction.Messaging;
using Application.Common.Results;
using Application.Features.Brand.Commands.CreateBrand;
using Application.Features.Brand.Commands.DeleteBrand;
using Application.Features.Brand.Commands.UpdateBrand;
using Application.Features.Brand.Queries.GetAllBrands;
using Application.Features.Brand.Queries.GetBrandById;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class BrandController : ControllerBase
{
    private readonly IMediator _mediator;

    public BrandController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Route(BrandRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetBrandByIdQuery { Id = id };
        Result<BrandDto> response = await _mediator.Send(query, cancellationToken);

        return response.ToActionResult();
    }

    [HttpGet]
    [Route(BrandRoutes.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var query = new GetAllBrandsQuery();
        var response = await _mediator.Send(query, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpPost]
    [Route(BrandRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateBrandCommand command, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(command, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpDelete]
    [Route(BrandRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteBrandCommand { Id = id };
        var response = await _mediator.Send(command, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpPut]
    [Route(BrandRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateBrandCommand request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBrandWithIdCommand
        {
            Id = id,
            Name = request.Name,
            ImageUrl = request.ImageUrl
        };

        var response = await _mediator.Send(command, cancellationToken);

        return response.ToActionResult();
    }
}
