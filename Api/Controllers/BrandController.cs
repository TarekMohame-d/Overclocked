using Api.ActionFilters;
using Api.Common.Routing;
using Api.Extensions;
using Application.Abstraction.Services;
using Application.Services.Brand.DTOs.Request;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class BrandController : ControllerBase
{
    private readonly IBrandService _brandServices;
    public BrandController(IBrandService brandServices)
    {
        _brandServices = brandServices;
    }

    [HttpGet]
    [Route(BrandRoutes.GetById)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetBrandByIdRequest { Id = id };
        var response = await _brandServices.GetBrandByIdAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    [HttpGet]
    [Route(BrandRoutes.GetAll)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var request = new GetAllBrandsRequest();
        var response = await _brandServices.GetAllBrandsAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpPost]
    [ServiceFilter(typeof(ValidationActionAttribute<CreateBrandRequest>))]
    [Route(BrandRoutes.Create)]
    public async Task<IActionResult> Create([FromBody] CreateBrandRequest request, CancellationToken cancellationToken)
    {
        var response = await _brandServices.CreateBrandAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpPut]
    [ServiceFilter(typeof(ValidationActionAttribute<UpdateBrandRequest>))]
    [Route(BrandRoutes.Update)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateBrandRequest request,
        CancellationToken cancellationToken)
    {
        request = request with { Id = id };

        var response = await _brandServices.UpdateBrandAsync(request, cancellationToken);

        return response.ToActionResult();
    }

    //[Authorize]
    [HttpDelete]
    [Route(BrandRoutes.Delete)]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new DeleteBrandRequest { Id = id };
        var response = await _brandServices.DeleteBrandAsync(request, cancellationToken);

        return response.ToActionResult();
    }
}
